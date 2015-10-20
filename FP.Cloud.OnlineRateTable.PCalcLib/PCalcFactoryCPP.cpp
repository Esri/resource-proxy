#include "ProductCalculation/src/PCalcManager.hpp"
#include "PCalcFactoryCPP.hpp"

using namespace ProductCalculation;

ProductCalculation::IPCalcManagerPtr PCalcFactory::GetPCalcMgr() const
{
	return m_pPCalcMgr;
}

ProductCalculation::IPCalcConfigurationPtr PCalcFactory::GetPCalcConfig() const
{
	return m_pPCalcMgr->GetPCalcConfig();
}

ProductCalculation::IPCalcActionManagerPtr PCalcFactory::GetPCalcActionMgr() const
{
	return m_pPCalcMgr->GetPCalcActionMgr();
}

ProductCalculation::IProductDescriptionPtr PCalcFactory::GetProdDesc() const
{
	return m_pPCalcMgr->GetProductDesc();
}

ProductCalculation::IPCalcMemoryKeyManagerPtr PCalcFactory::GetPCalcMemoryKeyMgr() const
{
	return m_pPCalcMgr->GetPCalcMemoryKeyMgr();
}

ProductCalculation::IPCalcGeneralInfoManagerPtr PCalcFactory::GetPCalcGeneralInfoMgr() const
{
	return m_pPCalcMgr->GetPCalcGeneralInfoMgr();
}

ProductCalculation::IPTMgrPtr PCalcFactory::GetPTMgr() const
{
	return m_pPCalcMgr->GetPTMgr();
}

ProductCalculation::IPCalcErrorLogManagerPtr PCalcFactory::GetPCalcErrorLogMgr() const
{
	return m_pPCalcMgr->GetPCalcErrorLogMgr();
}

ProductCalculation::IPCalcReportManagerPtr PCalcFactory::GetPCalcReportMgr() const
{
	return m_pPCalcMgr->GetPCalcReportMgr();
}

PCalcFactory::PCalcFactory()
	: m_pPCalcMgr(new ProductCalculation::PCalcManager())
{
}

PCalcFactory::~PCalcFactory()
{
}


