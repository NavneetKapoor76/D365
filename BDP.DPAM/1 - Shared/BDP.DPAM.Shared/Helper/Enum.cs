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

    public enum Location_StatusCode
    {
        Active = 1,
        Inactive = 2
    }

    public enum Contact_StateCode
    {
        Active = 0,
        Inactive = 1
    }

    public enum Contact_StatusCode
    {
        Active = 1,
        Inactive = 2
    }

    public enum Department_StateCode
    {
        Active = 0,
        Inactive = 1
    }

    public enum Department_StatusCode
    {
        Active = 1,
        Inactive = 2
    }

    public enum ContactFrequency_StateCode
    {
        Active = 0,
        Inactive = 1
    }

    public enum ContactFrequency_StatusCode
    {
        Active = 1,
        Inactive = 2
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

    public enum PhoneCall_StatusCode
    {
        Open = 1,
        Made = 2,
        Received = 4,
        Canceled = 3
    }

    public enum Appointment_StatusCode
    {
        Free = 1,
        Tentative = 2,
        Completed = 3,
        Canceled = 4,
        Busy = 5,
        OutOfOffice = 6
    }

    public enum Account_StatusCode
    {
        Active = 1,
        Prospect = 100000000,
        Inactive = 2,
        ComplianceReason = 100000001
    }

    public enum Account_StateCode
    {
        Active = 0,
        Inactive = 1
    }

    public enum Opportunity_StateCode
    {
        Open = 0,
        Won = 1,
        Lost = 2
    }

    public enum Opportunity_StatusCode
    {
        InProgress = 1,
        OnHold = 2,
        Won = 3,
        Canceled = 4,
        LostToCompetition = 5
    }
}
