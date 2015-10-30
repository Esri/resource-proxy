#pragma once

#include "PCalcLib.hpp"
#include "PCalcProxy.hpp"
#include "IPCalcProxy.hpp"
#include "Lock.hpp"

BEGIN_PCALC_LIB_NAMESPACE

public ref class PCalcProxyContext : System::IDisposable
{
public:
	/// <summary>
	/// Creates a instance of type PCalcProxyContext
	/// </summary>
	PCalcProxyContext(System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles);

	/// <summary>
	/// Creates a instance of type PCalcProxyContext
	/// </summary>
	PCalcProxyContext(IPCalcManager^ manager, IEnvironmentProcessor^ envProcessor, IActionResultProcessor^ actionProcessor,	ICalculationResultProcessor^ calcProcessor, IProductDescriptionMapper^ mapper,	System::String^ amxPath, System::String^ tablePath,	... array<System::String^>^ additionalFiles);

	~PCalcProxyContext(void);

	property IPCalcProxy^ Proxy
	{
		IPCalcProxy^ get()
		{
			Lock lock(PCalcProxy::SyncLock);
			if (m_IsInitialized == false)
				this->Init();
			return m_Proxy;
		}
	}


protected:
	!PCalcProxyContext(void);


private:
	void Init();

private:
	IPCalcProxy^ m_Proxy;
	PCalcFactory^ m_Factory;
	System::String^ m_AmxPath;
	System::String^ m_TablePath;
	bool m_IsInitialized;
};

END_PCALC_LIB_NAMESPACE