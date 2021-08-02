﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace Librame.Extensions
{
    public class ExpressionExtensionsTests
    {
        [Fact]
        public void PropertyTest()
        {
            var bodyName = nameof(Encoding.BodyName);

            Expression<Func<Encoding, string>> expression = p => p.BodyName;

            var propertyName = expression.AsPropertyName();
            Assert.Equal(bodyName, propertyName);

            var utf8Name = expression.AsPropertyValue(EncodingExtensions.UTF8Encoding);
            Assert.Equal("utf-8", utf8Name);

            // p => p.BodyName
            var createExpression = bodyName.CreatePropertyExpression<Encoding>();
            Assert.Equal(expression.ToString(), createExpression.ToString());
        }

        [Fact]
        public void CreatePropertyExpressionTest()
        {
            Expression<Func<Encoding, int>> expression = p => p.CodePage;

            var codePageName = nameof(Encoding.CodePage);

            var codePage = expression.AsPropertyValue(EncodingExtensions.UTF8Encoding);
            var orginalCodePage = 65001;

            // p => p.CodePage > orginalCodePage
            var greaterThanExpression = codePageName.CreateGreaterThanPropertyExpression<Encoding>(orginalCodePage);
            Assert.False(greaterThanExpression.Compile().Invoke(EncodingExtensions.UTF8Encoding));

            // p => p.CodePage >= orginalCodePage
            var greaterThanOrEqualExpression = codePageName.CreateGreaterThanOrEqualPropertyExpression<Encoding>(orginalCodePage);
            Assert.True(greaterThanOrEqualExpression.Compile().Invoke(EncodingExtensions.UTF8Encoding));

            // p => p.CodePage < orginalCodePage
            var lessThanExpression = codePageName.CreateLessThanPropertyExpression<Encoding>(orginalCodePage);
            Assert.False(lessThanExpression.Compile().Invoke(EncodingExtensions.UTF8Encoding));

            // p => p.CodePage <= orginalCodePage
            var lessThanOrEqualExpression = codePageName.CreateLessThanOrEqualPropertyExpression<Encoding>(orginalCodePage);
            Assert.True(lessThanOrEqualExpression.Compile().Invoke(EncodingExtensions.UTF8Encoding));

            // p => p.CodePage != orginalCodePage
            var notEqualExpression = codePageName.CreateNotEqualPropertyExpression<Encoding>(orginalCodePage);
            Assert.False(notEqualExpression.Compile().Invoke(EncodingExtensions.UTF8Encoding));

            // p => p.CodePage == orginalCodePage
            var equalExpression = codePageName.CreateEqualPropertyExpression<Encoding>(orginalCodePage);
            Assert.True(equalExpression.Compile().Invoke(EncodingExtensions.UTF8Encoding));
        }

        [Fact]
        public void NewTest()
        {
            var list = ExpressionExtensions.New<List<string>>();
            Assert.NotNull(list);

            var guid = Guid.NewGuid();
            var bytesGuid = ExpressionExtensions.New<Guid>(guid.ToByteArray());
            Assert.Equal(guid, bytesGuid);

            var stringGuid = (Guid)typeof(Guid).NewByExpression(guid.ToString());
            Assert.Equal(guid, stringGuid);
        }

    }
}
