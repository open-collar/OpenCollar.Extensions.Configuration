using System;
using TechTalk.SpecFlow;

namespace OpenCollar.Extensions.Configuration.Specs
{
    [Binding]
    public class InterfaceImplementationSteps
    {
        [Given(@"a property name")]
        public void GivenAPropertyName()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"a sample value")]
        public void GivenASampleValue()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"the value of the property is read")]
        public void WhenTheValueOfThePropertyIsRead()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the result must equal the sample value")]
        public void ThenTheResultMustEqualTheSampleValue()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
