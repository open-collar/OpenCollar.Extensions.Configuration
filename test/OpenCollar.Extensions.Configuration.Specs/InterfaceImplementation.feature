Feature: Interface Implementation
    In order to access configuration values
    As a configuration consumer
    I want to access configuration values through a strongly-typed interface

@interface
Scenario: Read an Int32 property value
	Given a property name
	And a sample value
	When the value of the property is read
	Then the result must equal the sample value