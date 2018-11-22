﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mailgun.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Mailgun.Tests.Extensions
{
    [TestClass]
    public class CollectionExtensionsSpec
    {
        [TestMethod]
        public void AddIfNotNullOrEmptyTest()
        {
            var dict = new Collection<KeyValuePair<string, string>>();

            dict.AddIfNotNullOrEmpty(string.Empty, string.Empty);
            dict.AddIfNotNullOrEmpty(string.Empty, "test");
            dict.AddIfNotNullOrEmpty("test", string.Empty);
            dict.AddIfNotNullOrEmpty(string.Empty, "");
            dict.AddIfNotNullOrEmpty("", string.Empty);


            dict.Count.ShouldBe(0);

            dict.AddIfNotNullOrEmpty("test", "test");

            dict.Count.ShouldBe(1);
        }
    }
}