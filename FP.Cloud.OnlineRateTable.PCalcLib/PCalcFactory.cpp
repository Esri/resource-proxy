#include "Base/misc/assert_m.h"
#include "PCalcFactory.hpp"
#include "PCalcFactoryCPP.hpp"


FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::PCalcFactory(void)
	: m_pFactory(new PT::PCalcFactory())
{

}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::~PCalcFactory(void)
{
	this->!PCalcFactory();
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::!PCalcFactory(void)
{
	if (NULL != m_pFactory)
	{		
		delete m_pFactory;
		m_pFactory = NULL;
	}
}

ProductCalculation::IPCalcConfigurationPtr FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::GetConfig(void)
{
	return m_pFactory->GetPCalcConfig();
}

ProductCalculation::IPCalcManagerPtr FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::GetPCalcMgr(void)
{
	return m_pFactory->GetPCalcMgr();
}

ProductCalculation::IProductDescriptionPtr FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::GetProdDesc(void)
{
	return m_pFactory->GetProdDesc();
}

ProductCalculation::IPCalcActionManagerPtr FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::GetActionMgr(void)
{
	return m_pFactory->GetPCalcActionMgr();
}

ProductCalculation::IPTMgrPtr FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::GetPTMgr(void)
{
	return m_pFactory->GetPTMgr();
}

ProductCalculation::IPCalcMemoryKeyManagerPtr FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::GetPCalcMemoryKeyMgr(void)
{
	return m_pFactory->GetPCalcMemoryKeyMgr();
}

ProductCalculation::IPCalcGeneralInfoManagerPtr FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::GetPCalcGeneralInfoMgr(void)
{
	return m_pFactory->GetPCalcGeneralInfoMgr();
}

ProductCalculation::IPCalcReportManagerPtr FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory::GetPCalcReportMgr(void)
{
	return m_pFactory->GetPCalcReportMgr();
}

