#include "ExtendedErrorCode.hpp"

BEGIN_PCALC_LIB_NAMESPACE

ExtendedErrorCode^ ConvertErrorCode(EXTENDED_ERROR_CODE error)
{
	ExtendedErrorCode^ managedError = gcnew ExtendedErrorCode(
		error.m_MainCode,
		error.m_Subcode1,
		error.m_Subcode2);

	return managedError;
}

END_PCALC_LIB_NAMESPACE