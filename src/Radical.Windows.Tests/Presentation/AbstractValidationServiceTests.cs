using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Validation;
using Radical.Windows.Presentation.Services.Validation;
using System.Collections.Generic;
using System.Linq;

namespace Test.Radical.Windows.Presentation
{
    [TestClass]
    public class AbstractValidationServiceTests
    {
        class TestValidationService : AbstractValidationService
        {
            readonly ValidationError[] errorsToReturnUnderTest;

            public TestValidationService( ValidationError[] errorsToReturnUnderTest )
            {
                this.errorsToReturnUnderTest = errorsToReturnUnderTest;
            }

            protected override IEnumerable<ValidationError> OnValidate()
            {
                return errorsToReturnUnderTest;
            }
        }

        [TestMethod]
        [TestCategory( "AbstractValidationService" ), TestCategory( "Validation" )]
        public void AbstractValidationService_validate_property_using_entity_with_non_valid_property_should_report_expected_errors()
        {
            var propName = "TestProperty";
            var expectedError = "--fake--";

            var expected = new[] { new ValidationError( propName, propName, new[] { expectedError } ) };
            var sut = new TestValidationService( expected );

            var result = sut.ValidateProperty( propName );

            Assert.AreEqual(result.Errors.Count(), expected.Length);
            Assert.AreEqual(expectedError, result.Errors.Single().DetectedProblems.Single());
            Assert.AreEqual(result.Errors.Single().PropertyName, expected[ 0 ].PropertyName);
        }

        //[TestMethod]
        //[TestCategory( "AbstractValidationService" ), TestCategory( "Validation" )]
        //public void AbstractValidationService_StatusChanged_event_should_be_triggered_each_time_errors_list_changes_even_if_validity_does_not_change()
        //{
        //    var actual = 0;

        //    var errors = new[]
        //    { 
        //        new ValidationError( "p1", "p1",new[] { "--fake--" } ),
        //        new ValidationError( "p2", "p2", new[] { "--fake--" } )
        //    };

        //    var sut = new TestValidationService( errors );
        //    sut.StatusChanged += ( s, e ) => actual += 1;

        //    sut.ValidateProperty( "p1" );
        //    sut.ValidateProperty( "p2" );
        //    sut.ValidateProperty( "p3" );

        //    Assert.AreEqual( 5, actual );
        //}

        //[TestMethod]
        //[TestCategory( "AbstractValidationService" ), TestCategory( "Validation" )]
        //public void AbstractValidationService_GetInvalidProperties_should_return_distinct_list()
        //{
        //    var errors = new[]
        //    { 
        //        new ValidationError( "p1", "p1", new[] { "--fake 1--" } ),
        //        new ValidationError( "p1", "p1", new[] { "--fake 2--" } ),
        //        new ValidationError( "p2", "p2", new[] { "--fake--" } )
        //    };

        //    var sut = new TestValidationService( errors );
        //    var invalid = sut.GetInvalidProperties();

        //    Assert.AreEqual( 2, invalid.Count() );
        //}
    }
}
