//#include "ProductCalculation/IPCalcManager.hpp"
#include "Exceptions.hpp"
#include "ProductCalculation/ProdCalcException.hpp"
#include "PCalcManager.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
using namespace PCalcManagedLib;

FP::Cloud::OnlineRateTable::PCalcLib::PCalcManager::PCalcManager(PCalcFactory^ factory)
	: m_Factory(factory)
{
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcManager::Create()
{
	try
	{
		m_Factory->GetPCalcMgr()->Create();
	}
	catch (ProdCalcException ex)
	{
		throw gcnew PCalcLibErrorCodeException(ex.GetErrorCode());
	}
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcManager::Initialize()
{
	try
	{
		m_Factory->GetPCalcMgr()->PCalcInitialize();
	}
	catch (ProdCalcException ex)
	{
		throw gcnew PCalcLibErrorCodeException(ex.GetErrorCode());
	}
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcManager::LoadPawn(System::String^ file)
{
	try
	{
		std::string cppPath(ConvertToString(file));
		boost::filesystem::path boostPath(cppPath);
		m_Factory->GetPCalcMgr()->LoadPawn(boostPath);
	}
	catch (ProdCalcException ex)
	{
		throw gcnew PCalcLibErrorCodeException(ex.GetErrorCode());
	}

}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcManager::LoadProductTable(System::String^ file)
{
	try
	{
		std::string cppPath(ConvertToString(file));
		m_Factory->GetPCalcMgr()->LoadProdTables(cppPath);
	}
	catch (ProdCalcException ex)
	{
		throw gcnew PCalcLibErrorCodeException(ex.GetErrorCode());
	}
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcManager::Unload()
{
	m_Factory->GetPCalcMgr()->Unload();
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcManager::CalculateStart([System::Runtime::InteropServices::Out] INT32 %rNextAction)
{
	int nextAction = 0;

	try
	{
		m_Factory->GetPCalcMgr()->PCalcCalculateProductStart(nextAction);
	}
	catch (ProdCalcException ex)
	{
		throw gcnew PCalcLibErrorCodeException(ex.GetErrorCode());
	}

	rNextAction = nextAction;
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcManager::CalculateNext([System::Runtime::InteropServices::Out] INT32 %rNextAction)
{
	int nextAction = 0;
	try
	{
		m_Factory->GetPCalcMgr()->PCalcCalculateProduct(nextAction);
	}
	catch (ProdCalcException ex)
	{
		throw gcnew PCalcLibErrorCodeException(ex.GetErrorCode());
	}

	rNextAction = nextAction;
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcManager::CalculateWeightChanged()
{
	m_Factory->GetPCalcMgr()->SetInputOperation(IPCalcManager::WEIGHT_CHANGED);

	int ignoreNextAction;
	this->CalculateNext(ignoreNextAction);
}


