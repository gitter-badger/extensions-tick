﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Librame.Extensions
{
    public class NullableExtensionsTests
    {

        [Fact]
        public void UnwrapTest()
        {
            Guid? g = null;
            Assert.Equal(Guid.Empty, g.Unwrap());
            Assert.NotEqual(Guid.Empty, g.Unwrap(Guid.NewGuid()));
        }


        #region IsNull and IsEmpty

        [Fact]
        public void IsNullAndIsNotNullTest()
        {
            NullableExtensionsTests? value = null;
            Assert.True(value.IsNull());

            value = new NullableExtensionsTests();
            Assert.True(value.IsNotNull());
        }

        [Fact]
        public void IsEmptyAndIsNotEmptyTest()
        {
            // IEnumerable
            IEnumerable? enumerable = Enumerable.Empty<int>();
            Assert.True(enumerable.IsEmpty());

            enumerable = Enumerable.Range(1, 5);
            Assert.True(enumerable.IsNotEmpty());

            // String
            var value = string.Empty;
            Assert.True(value.IsEmpty());

            value = "123";
            Assert.True(value.IsNotEmpty());
        }

        #endregion


        #region NotNull and NotEmpty

        [Fact]
        public void NotNullAndNotEmptyTest()
        {
            // Guid
            string? g = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                g.NotNull(nameof(g));
            });

            g = string.Empty;
            Assert.NotNull(g.NotNull(nameof(g)));

            // IEnumerable
            IEnumerable<int>? enumerable = Enumerable.Empty<int>();
            Assert.Throws<ArgumentException>(() =>
            {
                enumerable.NotEmpty(nameof(enumerable));
            });

            enumerable = Enumerable.Range(1, 5);
            Assert.NotNull(enumerable.NotEmpty(nameof(enumerable)));

            // String
            string str = string.Empty;
            Assert.Throws<ArgumentException>(() =>
            {
                str.NotEmpty(nameof(str));
            });

            str = " ";
            Assert.Throws<ArgumentException>(() =>
            {
                str.NotWhiteSpace(nameof(str));
            });

            str = "123";
            Assert.NotNull(str.NotEmpty(nameof(str)));
        }

        #endregion

    }
}
