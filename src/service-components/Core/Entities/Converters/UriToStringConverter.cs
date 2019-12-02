using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Umoya.Core.Entities
{
    public class UriToStringConverter : ValueConverter<Uri, string>
    {
        public static readonly UriToStringConverter Instance = new UriToStringConverter();

        public UriToStringConverter()
            : base(
                v => v.AbsoluteUri,
                v => new Uri(v))
        {
        }
    }
}
