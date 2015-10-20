#pragma once

#include "PCalcProxy.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

public ref class PCalcProxyContext
{
public:
	PCalcProxyContext(System::String^ amxPath, System::String^ tablePath);
	~PCalcProxyContext(void);

	property IPCalcProxy^ Proxy
	{
		IPCalcProxy^ get()
		{
			return m_Proxy;
		}
	}


protected:
	!PCalcProxyContext(void);

private:
	System::AppDomain^ m_Domain;
	PCalcProxy^ m_Proxy;
};

END_PCALC_LIB_NAMESPACE