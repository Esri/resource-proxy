#pragma once

#include <windows.h>
#include "Base/Error/ExtendedErrorCode.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace System;
using namespace System::Runtime::Serialization;

public ref class PCalcLibException : public Exception
{
public:
	PCalcLibException() : Exception()
	{		
	}

	PCalcLibException(String^ message) : Exception(message)
	{
	}

	PCalcLibException(String^ message, Exception^ innerException) : Exception(message, innerException)
	{
	}

protected:
	PCalcLibException(SerializationInfo^ info, StreamingContext context) : Exception(info, context)
	{		
	}
};

[Serializable]
public ref class PCalcLibErrorCodeException : public PCalcLibException
{
	
private:
	INT32 m_MainCode;
	INT32 m_Subcode1;
	INT32 m_Subcode2;

	static String^ MAIN_CODE = "MAINCODE";
	static String^ SUB_CODE1 = "SUBCODE1";
	static String^ SUB_CODE2 = "SUBCODE2";

public:
	//PCalcLibErrorCodeException() : PCalcLibException()
	//	, m_MainCode(0)
	//	, m_Subcode1(0)
	//	, m_Subcode2(0)
	//{}

	PCalcLibErrorCodeException(const INT32 &mainCode, const INT32 &subCode1, const INT32 &subCode2 ) 
		: PCalcLibException(System::String::Format("MainCode: 0x{0:X}, SubCode1: 0x{1:X}, SubCode2: 0x{2:X}", mainCode, subCode1, subCode2))
		, m_MainCode(mainCode)
		, m_Subcode1(subCode1)
		, m_Subcode2(subCode2)
	{		
	}

	//PCalcLibErrorCodeException(System::String^ message, Exception^ innerException) : PCalcLibException(message, innerException)
	//	, m_MainCode(0)
	//	, m_Subcode1(0)
	//	, m_Subcode2(0)
	//{
	//}

	PCalcLibErrorCodeException(const EXTENDED_ERROR_CODE &error) 
		: PCalcLibErrorCodeException(error.m_MainCode, error.m_Subcode1, error.m_Subcode2)

	{}

	virtual void GetObjectData(SerializationInfo^ info, StreamingContext context) override
	{
		if(nullptr == info)
		{
			throw gcnew ArgumentNullException("info");
		}

		info->AddValue(MAIN_CODE, m_MainCode);
		info->AddValue(SUB_CODE1, m_Subcode1);
		info->AddValue(SUB_CODE2, m_Subcode2);

		PCalcLibException::GetObjectData(info, context);
	}


	property INT32 MainCode { INT32 get() { return m_MainCode; } }
	property INT32 Subcode1 { INT32 get() { return m_Subcode1; } }
	property INT32 Subcode2 { INT32 get() { return m_Subcode2; } }

protected:
	PCalcLibErrorCodeException(SerializationInfo^ info, StreamingContext context) : PCalcLibException(info, context)
	{
		this->m_MainCode = info->GetInt32(MAIN_CODE);
		this->m_Subcode1 = info->GetInt32(SUB_CODE1);
		this->m_Subcode2 = info->GetInt32(SUB_CODE2);
	}

};

END_PCALC_LIB_NAMESPACE