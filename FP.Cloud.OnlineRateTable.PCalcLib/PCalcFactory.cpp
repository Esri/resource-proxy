#include "PCalcFactory.hpp"


USING_PCALC_LIB_NAMESPACE

PCalcFactory::PCalcFactory(void)
	: m_pFactory(new ProductCalculation::PCalcFactory())
{

}

PCalcFactory::~PCalcFactory(void)
{
	this->!PCalcFactory();
}

PCalcFactory::!PCalcFactory(void)
{
	delete m_pFactory;
}

ProductCalculation::IPCalcConfigurationPtr PCalcFactory::GetConfig(void)
{
	return m_pFactory->GetPCalcConfig();
}

ProductCalculation::IPCalcManagerPtr PCalcFactory::GetPCalcMgr(void)
{
	return m_pFactory->GetPCalcMgr();
}

ProductCalculation::IProductDescriptionPtr PCalcFactory::GetProdDesc(void)
{
	return m_pFactory->GetProdDesc();
}

ProductCalculation::IPCalcActionManagerPtr PCalcFactory::GetActionMgr(void)
{
	return m_pFactory->GetPCalcActionMgr();
}

ProductCalculation::IPTMgrPtr PCalcFactory::GetPTMgr(void)
{
	return m_pFactory->GetPTMgr();
}

ProductCalculation::IPCalcMemoryKeyManagerPtr PCalcFactory::GetPCalcMemoryKeyMgr(void)
{
	return m_pFactory->GetPCalcMemoryKeyMgr();
}

ProductCalculation::IPCalcGeneralInfoManagerPtr PCalcFactory::GetPCalcGeneralInfoMgr(void)
{
	return m_pFactory->GetPCalcGeneralInfoMgr();
}

ProductCalculation::IPCalcReportManagerPtr PCalcFactory::GetPCalcReportMgr(void)
{
	return m_pFactory->GetPCalcReportMgr();
}

