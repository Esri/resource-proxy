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
	PCalcProxyContext(Shared::EnvironmentInfo^ environment, System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles);

	/// <summary>
	/// Creates a instance of type PCalcProxyContext
	/// </summary>
	PCalcProxyContext(Shared::EnvironmentInfo^ environment, IPCalcManager^ manager, IEnvironmentProcessor^ envProcessor, IActionResultProcessor^ actionProcessor, ICalculationResultProcessor^ calcProcessor, IProductDescriptionMapper^ mapper, System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles);

	~PCalcProxyContext(void);

	property IPCalcProxy^ Proxy
	{
		IPCalcProxy^ get()
		{
			this->Init();
			return m_Proxy;
		}
	}


protected:
	!PCalcProxyContext(void);


private:
	void Init();

private:
	Shared::EnvironmentInfo^ m_Environment;
	PCalcProxy^ m_Proxy;
	System::String^ m_AmxPath;
	System::String^ m_TablePath;
	bool m_IsInitialized;
	Lock^ m_Lock;
	static System::Object^ m_SyncLock = gcnew System::Object();
};

END_PCALC_LIB_NAMESPACE