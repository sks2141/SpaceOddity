Feature: NumeralCalculator
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@NumeralCalculator
Scenario: Process Files
	Given Bootstrap Dependencies
	And Input From "TestFiles\TestFile_Input.txt"
	And Expected Output From "TestFiles\Test_ExpectedOutput.txt"
	When Processed by Calculator
	Then Compare Output To Expected Output