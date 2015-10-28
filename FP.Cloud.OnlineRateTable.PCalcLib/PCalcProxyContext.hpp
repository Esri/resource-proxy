#pragma once

#include "PCalcLib.hpp"
#include "PCalcProxy.hpp"
#include "IPCalcProxy.hpp"

BEGIN_PCALC_LIB_NAMESPACE

public ref class PCalcProxyContext
{
public:
	PCalcProxyContext(System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles);
	~PCalcProxyContext(void);

	property IPCalcProxy^ Proxy
	{
		IPCalcProxy^ get()
		{
			if(nullptr == m_Proxy)
			{
				this->Init();
			}
			return m_Proxy;
		}
	}


protected:
	!PCalcProxyContext(void);

private:
	void Init();

private:
	System::String^ m_AmxPath;
	System::String^ m_TablePath;
	System::AppDomain^ m_Domain;
	PCalcProxy^ m_Proxy;
};

END_PCALC_LIB_NAMESPACE