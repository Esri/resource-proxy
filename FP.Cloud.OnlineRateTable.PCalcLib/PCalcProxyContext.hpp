#pragma once

#include "PCalcProxy.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

public ref class PCalcProxyContext
{
public:
	PCalcProxyContext(void);
	~PCalcProxyContext(void);

	property PCalcProxy^ Proxy
	{
		PCalcProxy^ get()
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