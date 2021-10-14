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

    public enum Language
    {
        English = 100000000,
        French = 100000001,
        German = 100000002,
        Spanish = 100000003,
        Italian = 100000004,
        Dutch = 100000005,
        Portuguese = 100000006
    }

    public enum Gender
    {
        Male = 100000000,
        Female = 100000001,
        NonBinary = 100000002,
        GeneralAddress = 100000003
    }

    public enum EventRegistration_StateCode
    {
        Active = 0,
        Inactive = 1
    }

    public enum EventRegistration_StatusCode
    {
        Active = 1,
        Inactive = 2,
        Canceled = 100000000
    }

    public enum RegistrationResponse_StateCode
    {
        Active = 0,
        Inactive = 1
    }

    public enum RegistrationResponse_StatusCode
    {
        Active = 1,
        Inactive = 2
    }

    public enum SessionRegistration_StateCode
    {
        Active = 0,
        Inactive = 1
    }

    public enum SessionRegistration_StatusCode
    {
        Attented = 1,
        Canceled = 2
    }
}
