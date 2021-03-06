﻿using DotVVM.Samples.Tests.Base;
using DotVVM.Testing.Abstractions;
using OpenQA.Selenium;
using Riganti.Selenium.Core;
using Xunit;
using Xunit.Abstractions;

namespace DotVVM.Samples.Tests.Feature
{
    public class BindingContextsTests : AppSeleniumTest
    {
        [Fact]
        public void Feature_BindingContexts_BindingContext()
        {
            RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_BindingContexts_BindingContext);
                browser.Wait(1000);

                var linkCount = browser.FindElements("a").Count;
                for (var i = 0; i < linkCount; i++)
                {
                    var link = browser.ElementAt("a", i);
                    link.Click();
                    browser.WaitFor(() => {
                        AssertUI.InnerTextEquals(browser.Single(".result"), link.GetInnerText());
                    }, 3000, 50);
                }
            });
        }

        [Fact]
        public void Feature_BindingContexts_CollectionContext()
        {
            RunInAllBrowsers(browser => {
                foreach (var a in new[] { "Client", "Server" })
                {
                    browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_BindingContexts_CollectionContext + $"?renderMode={a}");
                    browser.Wait(1000);

                    var elements = browser.FindElements(By.ClassName("collection-index"));
                    elements.ThrowIfSequenceEmpty();
                    elements.ForEach(e => AssertUI.InnerTextEquals(e, elements.IndexOf(e).ToString()));
                }
            });
        }

        public BindingContextsTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}
