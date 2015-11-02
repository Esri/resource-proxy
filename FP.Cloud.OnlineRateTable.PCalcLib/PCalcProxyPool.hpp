#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcProxy;

private ref class PCalcProxyPool
{
public:
	~PCalcProxyPool();

	PCalcProxy^ Pop(Shared::EnvironmentInfo^ key);
	void Push(Shared::EnvironmentInfo^ key, PCalcProxy^ proxy);

	static property PCalcProxyPool^ Instance { PCalcProxyPool^ get() { return m_Singleton; } };

protected:
	!PCalcProxyPool();

private:	
	PCalcProxyPool();
	int GetKeyValue(Shared::EnvironmentInfo^ key);
	System::Collections::Generic::KeyValuePair<int, PCalcProxy^>^ m_Pool;

	static PCalcProxyPool^ m_Singleton = gcnew PCalcProxyPool();
};


END_PCALC_LIB_NAMESPACE