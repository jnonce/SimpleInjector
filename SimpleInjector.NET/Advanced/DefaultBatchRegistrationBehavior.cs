﻿#region Copyright Simple Injector Contributors
/* The Simple Injector is an easy-to-use Inversion of Control library for .NET
 * 
 * Copyright (c) 2015 Simple Injector Contributors
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
 * associated documentation files (the "Software"), to deal in the Software without restriction, including 
 * without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO 
 * EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

namespace SimpleInjector.Advanced
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using SimpleInjector.Decorators;

    [DebuggerDisplay("{GetType().Name,nq}")]
    internal sealed class DefaultBatchRegistrationBehavior : IBatchRegistrationBehavior
    {
        private readonly Container container;

        public DefaultBatchRegistrationBehavior(Container container)
        {
            this.container = container;
        }

        public bool ShouldRegisterType(Type serviceType, Type implementationType)
        {
            return
                !this.IsDecorator(serviceType, implementationType) &&
                !IsContractClass(serviceType, implementationType);
        }

        private bool IsDecorator(Type serviceType, Type implementationType)
        {
            return DecoratorHelpers.IsDecorator(this.container, serviceType, implementationType);
        }

        private static bool IsContractClass(Type serviceType, Type implementationType)
        {
            // NOTE: The ContractClassForAttribute is marked with [Conditional("CONTRACTS_FULL")].
            // This means that without the CONTRACTS_FULL compiler directive, a class marked with this
            // attribute will just become a regular class and will end up as a normal class, and it will be
            // included.
            var attributes = implementationType.GetCustomAttributes(typeof(ContractClassForAttribute), true);

            var contractAttributesForServiceType =
                from ContractClassForAttribute attribute in attributes
                where attribute.TypeContractsAreFor == serviceType
                select attribute;

            return contractAttributesForServiceType.Any();
        }
    }
}