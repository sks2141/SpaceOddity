I. How to run:
	A. Pre-requisites:
	- Navigate to the extracted folder and open "SpaceOddity.sln" using Visual Studio or any other IDE
	- Build solution
	
	B. 
	i. Run via ClientConsole's bin directory
	- Navigate to ClientConsole's bin directory 
	- This contains TestData Folder: To add your text file, paste into in here
	- Open ClientConsole.exe.config and update "InputFilePath" configuration to point to the file that you want to test the application with 
	- Run ClientConsole.exe
	
	ii. Run via Visual Studio or any other IDE
	- Open App.config & Edit "InputFilePath" configuration to point to the file that you want to test the application with 
	- Run ClientConsole project
=================================================================================
II. About Codebase: 
    A. External libraries/tools:
	- Autofac IOC container for dependency management & run-time resolution of injected dependencies. Dependencies are modularly built inside each consumed class library via a "Module" suffixed file. The ClientConsole stitches the entire dependency chain via "DependencyBuilder" class.
	
	- log4net to address logging cross cutting concerns. SLogger is a static logging wrapper on the implementation to help instrument/troubleshoot/debug issues. These are referenced in projects based on a 'log4net.config' file (referenced via App.config) that spins out 2 logs - a verbose and a less verbose one. The 'smtp' based logging is not enabled for this assignment.
	
	- FluentValidation for easier readability of validation rules and ease of maintainability
	
	- NUnit & Moq for Unit testing purposes & tool: NUnit Test Adapter for test execution. There were 12,145 unit tests written.
	
	- SpecFlow for BDD testing & SpecFlow for visual studio add-in [Please Note: This was not fully baked in]. The test associated to this fail in the Test Explorer.
	
	B. Project Structure & Purpose
    - Common : to share components ex. App.config, log4net.config
	
	- Core : to share Data Access, Utilies and Business layers
	
		- "No Domain Lib": The assignment needs to read and interpret file contents and then evaluate queries, which are simple data structures. I did not find it purposeful to dedicate a library for holding to a Model layer. For future scalability, the data structures used can be embedded in here.
		
		- DataAccessLib: A repository (Data Access Layer) to vend out data.
		
		- Utilites : For Cross-cutting concerns and sharable snippets such as a FileReader to allow reading the file without locking the files. By default, it cleans the extra whitespaces in the files, to minimize massaging.
		
		- ValidationLib : For hold minimalistic business-level validation in place
		
		- ValidationLib.UnitTests : To solidify each validator in ValidationLib
		
	- Engine : Heart of Processing
	
		- NumeralCalculator: Consists of Algorithm to read an input file, interpret its contents and answer the questions. Further sub-divided into 
			- Worker : Orchestrator of moving parts. Processor delegates the the task of pre-processing the file and analyzing the processed input to concerned workers. This uses Validators and Interpreters internally layout different assumptions.
			
			- Validator : Validate the input data, specialized by identifying it as data related to Product details or related to Query. A compound validator is referenced by the Interpreters, at a later phase, to validate based on what was interpreted/learnt earlier.
			
			- Interpreter : Interpret input data to evaluate Product details and Query details 
			
			- Converter : To strategize conversion between different units
			
		- NumeralCalculator.Specs : A BDD based project, albeit not fully baked in, has a good potential to collaborate with other team members, if required.

		- NumeralCalculator.UnitTests : To solidify the logic used in NumeralCalculator
		
	- ClientConsole : Project to run
		Note: I decided to limit the presentation layer with basic console application. 
=================================================================================
III. Assumptions: Business & Design:
    - Input is via a Text file, referenced via ClientConsole's app.config.
	
	- Product catalog of Earthy vs. Intergalactic products are vended via EarthyProducts.txt & IntergalacticProducts.txt files [See Common folder]. These are used by the 'ProductCategorizer' to identify ProductType & validation the product, both during the pre-processing step (via validators) and during the analysis step (via interpreters). Note that these entries are case sensitive and its advisable to use the same case sensitivity in the input file [I admit - I did not thoroughly test this out]
	
	- ProductHelper helps to bridge missing data interpretations, shared by the interpreters.
	
	- Validation rules mentioned in the requirements are encapsulated by RuleEngineValidation, which localizes business logic to each Numeral based validator. RomanNumeralValidatorTest further details both the valid and the invalid data points, abiding to the requirements, with a total of '4010' unit tests.
	
	- Converter strategy holds a referential set of RomanNumeral to ArabicNumeral maps and reverse map for ease of conversion. Conversion to Roman Numeral from an integer is restricted from 1-3999, primarily to stick with the validation requirements provided in the assignment. This constraint on the Arabic Numeral is relaxed since it failed processing the input data line for 'glob prok Gold is 57800 Credits'. Converter strategy's unit tests run through {staggering} - '8006' combinations to correctly convert data.
		
	- Validators - ProductDataValidator & QueryValidator share the list of products. Due to synchronous nature of processing, in the pre-processing step, the ProductDataValidator creates a knowledge of all the products it encounters, with help of ProductCategorizer. This is then used by QueryValidator to root out Questions, for which contains "unknown" products. 
	(An improvement will be to write such unknown values to a cache and later process it, after the first time processing)
		
	- Interpreters - ProductDataInterpreter & QueryInterpreters share  intergalacticProductCache & earthyProductsCache. Each in-memory cache contains a valid 'interpreted' entry. 
		- Earthy Products have decimal values, stored in earthyProductsCache. Since these are interpreted by input line, they can potentially not have pure number value. 
		ex. Iron value from the below lines is 195.5 - a decimal. 
			pish is X
			pish pish Iron is 3910 Credits
			
		- These decimal values are formatted to two places in QueryInterpeter. A decimal value with zero fraction is removed from the representation.
					
		- Intergalactic Products have values in Roman numeral, stored in intergalacticProductsCache. This is used by the ProductHelper to help get multipliers, to infer the absolute value of Earthy products.
			
	- Compound validator furthers polices additional validation at the interpreter level. It looks for validity of a product, based on:
		- For input text product data, multiple Intergalactic products cannot exist, without any Earthy products tagged
		- Intergalactic products cannot have values in Arabic Numeral. Only Roman Numerals are valid.
		- Earthy products cannot have values in Roman numeral. Only Arabic Numerals are valid.
		- Multiple Earthy products entries cannot be mentioned in a single lie
		- Earthy products should be mentioned at the end of the product list, when tagged along with intergalactic products
		
		- For a complete list, refer CompoundValidatorTest unit test file.
	
	- Workflow:
	a. A runner in ClientConsole app calls the Processor, which sequentially preprocess each line in the file and then analyses the input, providing the output. Each line processing is decorated using a try-catch block to gulp the bubbled exception and continue processing to the next line. 
	Note: Unhandled exception are logged at the ClientConsole level.
	
	b. The Preprocessor uses Validators (via ctor injection), to massage and output a list of Tuple containing 
		- Input : File Contents 
		
		- Basic File Line Parsing Assumptions (in preprocessing step):
			- Each line should have a {" is "} to raise the interest of interpretation
			- Query input type is identified by a "?" at the end of the line
			- Earthy products derive their value via input that have 'Credits'. If a line detailing Earthy product assignment to a value does not contain Credits, its processing will be skipped.
		
		- Additional memory footprint : list of products noticed by the validators
		
	    - Output: List of 
			- Line Number
			- Input Type : Product Data (assignment) or Query
			- List of Products 
			- Potential value for the targeted 'Product'
			
	c. The Analyzer funnels the interpretation of data based on the 'Product Type'. If the list of products is empty in the 'Product Data' input type (i.e. assignment of products), it skips the intrepretation of products; however, the query interpretation is never skipped.
		- Input: Output of the PreProcessor
			
		- Additional memory footprint : IntergalacticProductsCache & EarthyProductsCache

		- Output: A list of Responses, by basic query interpretation based on knowledge it parsed.		

=================================================================================
IV. Thoughts
	- Current:
		- Scalability: The solution can easily be scaled. Current procesing is synchronous to allow for easier derivation of input and naturally process for output. 
		
		- Design: Designing a distributed system and/or using threading with r/w locks, I thought, was an overkill to the task.
		
		- Reliability: To increase reliability, the application can easily be extended to write the caches / lists to file/other I/O sources such as database and later bootstrap the file on startup, since these structures are injected into each consumer.
		
		- Performance Testing: Honestly, I did not get enough time to benchmark my code performance.
		
		- UI Presentation : This can make the algorithm more resourceful and  better user experience. With the current implementation, due to the synchronity of algorithm, using Deferred execution or Async-await processing did not sound justifiable to me.
	
	- General:
		- This assignment reminded me of "Space Odyssey 2001" movie and the song "Space Oddity" by David Bowie. Hence, the name of the solution.
		
		- Imagining that if I ever live such a life and if this application was the requirement to the Buy/Sell business, I would definitely incorporate some of the 'Current' Thoughts and distribute the app to fellow merchants and other consumers. This will help get feedbacks, to derive a better product.	