#include "PCalcManager.hpp"
#include "PCalcProxyContext.hpp"

BEGIN_PCALC_LIB_NAMESPACE

PCalcProxyContext::PCalcProxyContext(System::String^ amxPath, System::String^ tablePath)
{
	System::AppDomainSetup^ setup = gcnew System::AppDomainSetup();
	setup->ApplicationBase = System::AppDomain::CurrentDomain->SetupInformation->ApplicationBase;
	setup->PrivateBinPath = System::AppDomain::CurrentDomain->SetupInformation->PrivateBinPath;

	m_Domain = System::AppDomain::CreateDomain(System::Guid::NewGuid().ToString(), nullptr, setup);
	m_Proxy = (PCalcProxy^)m_Domain->CreateInstanceAndUnwrap(System::Reflection::Assembly::GetExecutingAssembly()->FullName, PCalcProxy::typeid->FullName);

	m_Proxy->Manager->Create();
	m_Proxy->Manager->LoadPawn(amxPath);
	m_Proxy->Manager->LoadProductTable(tablePath);
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::~PCalcProxyContext(void)
{
	this->!PCalcProxyContext();
}

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

END_PCALC_LIB_NAMESPACE

