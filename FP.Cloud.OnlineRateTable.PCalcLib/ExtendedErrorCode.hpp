#pragma once

#include <windows.h	>
#include "Base/Error/ExtendedErrorCode.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

	[System::Serializable]
	public ref class ExtendedErrorCode
	{
	private:
		INT32	m_MainCode;
		INT32	m_Subcode1;
		INT32	m_Subcode2;

	public:
		ExtendedErrorCode()
			: m_MainCode(0)
			, m_Subcode1(0)
			, m_Subcode2(0)
		{
		}

		ExtendedErrorCode(INT32 mainCode, INT32 subcode1, INT32 subcode2)
			: m_MainCode(mainCode)
			, m_Subcode1(subcode1)
			, m_Subcode2(subcode2)
		{
		}

		property INT32 MainCode { INT32 get() { return m_MainCode; } }

		property INT32 Subcode1 { INT32 get() { return m_Subcode1; } }

		property INT32 Subcode2 { INT32 get() { return m_Subcode2; } }

		static operator bool(ExtendedErrorCode^ errorCode)
		{
			return errorCode->MainCode != 0;
		}
	};

	ExtendedErrorCode^	ConvertErrorCode(EXTENDED_ERROR_CODE error);

END_PCALC_LIB_NAMESPACE