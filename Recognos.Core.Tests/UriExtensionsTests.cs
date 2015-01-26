using System;
using System.Linq;
using System.Net;
using FluentAssertions;
using Recognos.Core;
using Xunit;

namespace Recognos.Test.Core
{

    public class UriExtensionsTests
    {
        [Fact]
        public void UriExtensions_ExtractUris_SimpleUri()
        {
            string input = "http://www.google.com";
            Uri expected = new Uri(input);

            input.ExtractUris().Single().Should().Be(expected);
        }

        [Fact]
        public void UriExtensions_ExtractUris_FromHtml()
        {
            string input = "<a href=\"http://www.google.com\"><other";
            Uri expected = new Uri("http://www.google.com");

            input.ExtractUris().Single().Should().Be(expected);
        }

        [Fact]
        public void UriExtensions_ExtractUris()
        {
            string input = " Support the awesome drummer Eddie Fisher of OneRepublic! Go to the Facebook page and click \"I like\"! http://www.facebook.com/pages/Eddie-Fisher-OneRepublic-F ...";
            Uri expected = new Uri("http://www.facebook.com/pages/Eddie-Fisher-OneRepublic-F");

            input.ExtractUris().Single().Should().Be(expected);
        }

        [Fact]
        public void UriExtensions_ExtractUris_WithoutHost()
        {
            string input = "{asd:}";
            input.ExtractUris().Any().Should().BeFalse();
        }

        [Fact]
        public void UriExtensions_IsWeb()
        {
            new Uri("http://www.google.com").IsWeb().Should().BeTrue();
            new Uri("https://www.google.com").IsWeb().Should().BeTrue();
            new Uri("ftp://www.google.com").IsWeb().Should().BeFalse();
            new Uri("nttp://www.google.com").IsWeb().Should().BeFalse();
        }

        [Fact]
        public void UriExtensions_ResolveRedirects_Facebook()
        {
            Uri uri = new Uri("http://t.co/6VyBZOO");
            Uri expected = new Uri("https://www.facebook.com/photo.php?pid=585685&l=588142298a&id=100399403341183");

            uri.ResolveRedirects().Should().Be(expected);
        }

        [Fact]
        public void UriExtensions_ResolveRedirects_ErrorHandling()
        {
            Uri uri = new Uri("http://notfound-asdadagasdg.com");

            Func<WebException, Uri> map = (x) => null;

            uri.ResolveRedirects(map).Should().BeNull();
        }

        [Fact]
        public void UriExtensions_ResolveRedirects_ThrowsOnNonWeb()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Uri uri = new Uri("ftp://www.google.com");
                uri.ResolveRedirects();
            });
        }

        [Fact]
        public void UriExtensions_ResolveRedirects_HEADnotAllowed()
        {
            Uri uri = new Uri("http://amzn.to/");
            Uri expected = new Uri(@"http://www.amazon.com/");
            Exception ex = null;
            Uri resolved = uri.ResolveRedirects(x => { ex = x; return uri; });

            ex.Should().BeNull();
            uri.ResolveRedirects().Should().Be(expected);
        }

        [Fact(Skip = "tinyurl uses meta tag for redirect ")]
        public void UriExtensions_ResolveRedirects_TinyUrl()
        {
            Uri uri = new Uri("http://tinyurl.com/3lesxnx");
            uri.ResolveRedirects().Should().Be(new Uri("http://edition.cnn.com/2011/WORLD/europe/07/18/uk.committee.hearing/"));
        }

    }

}
