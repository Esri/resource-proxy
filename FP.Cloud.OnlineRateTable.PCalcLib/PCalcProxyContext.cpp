#include "PCalcProxyContext.hpp"

BEGIN_PCALC_LIB_NAMESPACE

PCalcProxyContext::PCalcProxyContext(void)
{
	System::AppDomainSetup^ setup = gcnew System::AppDomainSetup();
	setup->ApplicationBase = System::AppDomain::CurrentDomain->SetupInformation->ApplicationBase;
	setup->PrivateBinPath = System::AppDomain::CurrentDomain->SetupInformation->PrivateBinPath;

	m_Domain = System::AppDomain::CreateDomain(System::Guid::NewGuid().ToString(), nullptr, setup);
	m_Proxy = (PCalcProxy^)m_Domain->CreateInstanceAndUnwrap(System::Reflection::Assembly::GetExecutingAssembly()->FullName, PCalcProxy::typeid->FullName);
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::~PCalcProxyContext(void)
{
	this->!PCalcProxyContext();
}

END_PCALC_LIB_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::!PCalcProxyContext(void)
{
	if (nullptr != m_Proxy)
	{
		delete m_Proxy;
	}

	if (nullptr != m_Domain)
	{
		System::AppDomain::Unload(m_Domain);
	}
}

