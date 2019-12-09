﻿using System;
using System.Collections.Generic;
using System.Linq;
using Radical.ComponentModel.Validation;
using Radical.Validation;

namespace Radical.Windows.Presentation.Services.Validation
{
	/// <summary>
	/// Defines a validation service specialized in entity validation.
	/// </summary>
	/// <typeparam name="T">The type of the entity to validate.</typeparam>
	public class EntityValidationService<T> : AbstractValidationService
	{
		readonly T entity;
		readonly IEnumerable<IValidator<T>> validators;

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityValidationService&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="validators">The validators.</param>
		public EntityValidationService( T entity, params IValidator<T>[] validators )
		{
			this.entity = entity;
			this.validators = validators;
		}

		/// <summary>
		/// Called in order to execute the concrete validation process.
		/// </summary>
		/// <param name="ruleSet">The rule set.</param>
		/// <returns>
		/// A list of <seealso cref="ValidationError"/>.
		/// </returns>
		protected override IEnumerable<ValidationError> OnValidate(string ruleSet )
		{
			//TODO: è sbagliato che venga preso un singolo validatore se ruleSet è null, dovrebbero essere presi tutti
			var validator = validators.SingleOrDefault( v => v.RuleSet == ruleSet );
			if( validator != null )
			{
				var result = validator.Validate( entity );
				return result.Errors;
			}

			return new ValidationError[ 0 ];
		}

		/// <summary>
		/// Called in order to execute the concrete validation process on the given property.
		/// </summary>
		/// <param name="ruleSet">The rule set.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		/// A list of <seealso cref="ValidationError" />.
		/// </returns>
		protected override IEnumerable<ValidationError> OnValidateProperty(string ruleSet, string propertyName )
		{
			var validator = validators.SingleOrDefault( v => v.RuleSet == ruleSet );
			if( validator != null )
			{
				var result = validator.Validate( entity, propertyName );
				if( !result.IsValid )
				{
					return result.Errors.Where( e => e.Key == propertyName );
				}
			}

			return new ValidationError[ 0 ];

			//return base.OnValidateProperty( ruleSet, propertyName );
		}
	}
}