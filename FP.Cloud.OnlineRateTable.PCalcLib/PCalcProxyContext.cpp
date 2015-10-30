#include "PCalcManager.hpp"
#include "PCalcProxyContext.hpp"
#include "Exceptions.hpp"
#include "PCalcLibNinjectModule.hpp"

BEGIN_PCALC_LIB_NAMESPACE

PCalcProxyContext::PCalcProxyContext(System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles)
	: m_Proxy(nullptr)
	, m_Factory(nullptr)
	, m_AmxPath(amxPath)
	, m_TablePath(tablePath)
	, m_Kernel(gcnew Ninject::StandardKernel(gcnew PCalcLibNinjectModule()))
{
	m_Kernel->Bind<Lib::PCalcFactory^>()->ToSelf()->InScope(gcnew System::Func<Ninject::Activation::IContext^, System::Object^>(this, &PCalcProxyContext::GetScope));
}

PCalcProxyContext::PCalcProxyContext(IPCalcManager^ manager, IEnvironmentProcessor^ envProcessor, IActionResultProcessor^ actionProcessor, ICalculationResultProcessorProxy^ calcProcessor, IProductDescriptionMapper^ mapper, System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles)
	: m_Proxy(nullptr)
	, m_Factory(nullptr)
	, m_AmxPath(amxPath)
	, m_TablePath(tablePath)
	, m_Kernel(gcnew Ninject::StandardKernel(gcnew PCalcLibNinjectModule()))
{
	m_Kernel->Rebind<IPCalcManager^>()->ToConstant(manager);
	m_Kernel->Rebind<IEnvironmentProcessor^>()->ToConstant(envProcessor);
	m_Kernel->Rebind<IActionResultProcessor^>()->ToConstant(actionProcessor);
	m_Kernel->Rebind<ICalculationResultProcessorProxy^>()->ToConstant(calcProcessor);
	m_Kernel->Rebind<IProductDescriptionMapper^>()->ToConstant(mapper);
}

System::Object^ PCalcProxyContext::GetScope(Ninject::Activation::IContext^ context)
{
	return this;
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::Init()
{
	m_Factory = Ninject::ResolutionExtensions::Get<Lib::PCalcFactory^>(m_Kernel);
	m_Proxy = Ninject::ResolutionExtensions::Get<Lib::IPCalcProxy^>(m_Kernel);

	PCalcProxy^ proxy = dynamic_cast<PCalcProxy^>(m_Proxy);

	if (nullptr != proxy)
	{
		try
		{
			proxy->Init(m_AmxPath, m_TablePath);
		}
		catch (System::Runtime::InteropServices::SEHException^ ex)
		{
			throw gcnew PCalcLibException(ex->Message, ex);
		}
	}
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
		m_Proxy = nullptr;
	}

	if (nullptr != m_Factory)
	{
		m_Kernel->Release(m_Factory);
		m_Factory = nullptr;
	}

	m_AmxPath = nullptr;
	m_TablePath = nullptr;
	m_Kernel = nullptr;
}

END_PCALC_LIB_NAMESPACE

