#include "ProductCalculation/ProdCalcException.hpp"
#include "ProductCalculation/ProdTableException.hpp"
#include "boost/filesystem/path.hpp"
#include "QueryCreationStrategy.hpp"
#include "QueryShowMenuCreationStrategy.hpp"
#include "Lock.hpp"
#include "PCalcProxy.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
USING_PCALC_LIB_NAMESPACE

using namespace PCalcManagedLib;

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::PCalcProxy()
	: m_Factory(gcnew PCalcFactory())
{
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::~PCalcProxy()
{
	this->!PCalcProxy();
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Create()
{
	EXTENDED_ERROR_CODE error(GENERAL_Error::SUCCESSFUL);

	try
	{
		m_Factory->GetPCalcMgr()->Create();
	}
	catch (ProdCalcException ex)
	{
		error = ex.GetErrorCode();
	}
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Initialize()
{
	EXTENDED_ERROR_CODE error(GENERAL_Error::SUCCESSFUL);
	try
	{
		m_Factory->GetPCalcMgr()->PCalcInitialize();
	}
	catch (ProdCalcException ex)
	{
		error = ex.GetErrorCode();
	}
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Initialize(EnvironmentInfo^ environment)
{
	this->SetEnvironment(environment);
	this->Initialize();
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::LoadPawn(System::String^ file)
{
	EXTENDED_ERROR_CODE error(GENERAL_Error::SUCCESSFUL);
	try
	{
		std::string cppPath(ConvertToString(file));
		boost::filesystem::path boostPath(cppPath);
		m_Factory->GetPCalcMgr()->LoadPawn(boostPath);
	}
	catch (ProdCalcException ex)
	{
		error = ex.GetErrorCode();
	}
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::LoadProductTable(System::String^ file)
{
	EXTENDED_ERROR_CODE error(GENERAL_Error::SUCCESSFUL);
	try
	{
		std::string cppPath(ConvertToString(file));
		m_Factory->GetPCalcMgr()->LoadProdTables(cppPath);
	}
	catch (ProdCalcException ex)
	{
		error = ex.GetErrorCode();
	}
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::CalculateStart([System::Runtime::InteropServices::Out] INT32 %rNextAction)
{
	EXTENDED_ERROR_CODE error(GENERAL_Error::SUCCESSFUL);
	int nextAction = 0;
	try
	{
		m_Factory->GetPCalcMgr()->PCalcCalculateProductStart(nextAction);
	}
	catch (ProdCalcException ex)
	{
		error = ex.GetErrorCode();
	}

	rNextAction = nextAction;
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, WeightInfo^ weight)
{
	Lock lock(this);
	int nextAction = 0;

	this->Initialize(environment);
	this->SetWeight(weight);
	this->CalculateStart(nextAction);
	return this->ProcessResult(nextAction);
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult)
{
	Lock lock(this);
	INT32 nextAction = 0;
	ProductCalculation::ActionResultType target;
	ProductCalculation::ResultValueVectorType results;

	this->Calculate(environment, product->Weight);

	target.ID = (int)actionResult->Action;
	target.Label = actionResult->Label;
	for each(AnyInfo^ current in actionResult->Results)
	{
		switch (current->AnyType)
		{
		case EAnyType::INT32:
			results.push_back(System::Convert::ToInt32(current->AnyValue));
			break;
		default:
			break;
		}
	}
	target.Result = results;

	ProductCalculation::IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	parameter->SetProductCode(product->ProductCode);
	parameter->SetProductID(product->ProductId);
	parameter->SetActionResult(target);

	this->CalculateNext(nextAction);
	return this->ProcessResult(nextAction);
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::CalculateNext([System::Runtime::InteropServices::Out] INT32 %rNextAction)
{
	EXTENDED_ERROR_CODE error(GENERAL_Error::SUCCESSFUL);
	int nextAction = 0;
	try
	{
		m_Factory->GetPCalcMgr()->PCalcCalculateProduct(nextAction);
	}
	catch (ProdCalcException ex)
	{
		error = ex.GetErrorCode();
	}

	rNextAction = nextAction;
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::!PCalcProxy()
{
	delete m_Factory;
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Unload()
{
	m_Factory->GetPCalcMgr()->Unload();
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::SetEnvironment(EnvironmentInfo^ environment)
{
	IPCalcConfigurationPtr config = m_Factory->GetConfig();
	IPropertiesPtr properties = config->AccessCurrentProperties();

	properties->SetValue(EnvironmentProperty::REQUEST_ORIGIN_ZIP_CODE, PCalcManagedLib::ConvertToString(environment->SenderZipCode));
	properties->SetValue(EnvironmentProperty::REQUEST_LANG, PCalcManagedLib::ConvertToString(environment->Culture));

	config->ChangeProperties(properties);
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::SetWeight(WeightInfo^ weight)
{
	ProductCalculation::IProductDescParameterPtr product = m_Factory->GetProdDesc()->AccessCurrProduct();
	ProductCalculation::WeightType newWeight(weight->WeightValue, (BYTE)weight->WeightUnit);
	product->SetWeight(newWeight);
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::ProcessResult(INT32 id)
{
	ActionID actionID = (ActionID)id;
	PCalcResultInfo^ result = gcnew PCalcResultInfo();
	QueryCreationStrategy^ strategy = nullptr;

	switch (id)
	{
	case ACTION_SHOW_MENU:
	{
		strategy = gcnew QueryShowMenuCreationStrategy(m_Factory);
		break;
	}
	default:
		throw gcnew System::InvalidOperationException();
	}

	strategy->SetResult(result);
	return result;
}

