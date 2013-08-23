using Postmaster.io.Api.V1.Entities.Helper;
using Postmaster.io.Api.V1.Entities.Shipment;

namespace Postmaster.io
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // new shipment
            var shipment = new Shipment
            {
                From = new From
                {
                    Country = "USA",
                    Contact = "A friend",
                    Line1 = "730 NW 23rd St",
                    City = "Oklahoma City",
                    State = "Oklahoma",
                    ZipCode = "73103",
                    PhoneNo = "123-123-1234",
                    Residential = true
                },
                To = new To
                {
                    Country = "USA",
                    Contact = "Jesse James",
                    Line1 = "727 NW 23rd St",
                    City = "Oklahoma City",
                    State = "Oklahoma",
                    ZipCode = "73103",
                    PhoneNo = "123-123-1234",
                    Residential = true
                },
                Package = new Package
                {
                    Weight = 1.5,
                    Length = 10,
                    Width = 6,
                    Height = 8
                },
                Carrier = Carrier.Ups,
                Service = Service.TwoDay
            };

            // create shipment
            shipment.Create();
        }
    }
}