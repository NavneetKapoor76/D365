using System;
using System.Collections.Generic;
using System.Text;

namespace BDP.DPAM.Shared.Helper
{
	//public enum ProjectContractStatus
	//{
	//	Draft = 192350000,
	//	InReview = 192350001,
	//	OnHold = 192350002,
	//	Confirmed = 192350003,
	//	Completed = 192350004,
	//	Lost = 192350005,
	//	Abandoned = 192350006
	//}

    public enum LocationStateCode
    {
        Active = 0,
        Inactive = 1
    }

    public enum Contact_Language
    {
        English = 100000000,
        French = 100000001,
        German = 100000002,
        Spanish = 100000003,
        Italian = 100000004,
        Dutch = 100000005,
        Portuguese = 100000006
    }

    public enum Contact_Gender
    {
        Male = 1,
        Female = 2,
        NonBinary = 100000000
    }

    public enum Greeting_Gender
    {
        Male = 100000000,
        Female = 100000001,
        NonBinary = 100000002
    }
}
