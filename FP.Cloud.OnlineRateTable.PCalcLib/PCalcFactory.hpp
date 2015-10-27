#pragma once

#include "ProductCalculation/IPCalcManager.hpp"
#include "ProductCalculation/IPCalcConfiguration.hpp"
#include "ProductCalculation/IProductDescription.hpp"
#include "ProductCalculation/IPCalcActionManager.hpp"
#include "ProductCalculation/IPTMgr.hpp"
#include "ProductCalculation/IPCalcMemoryKeyManager.hpp"
#include "ProductCalculation/IPCalcGeneralInfoManager.hpp"
#include "ProductCalculation/IPCalcReportManager.hpp"
#include "PCalcLib.hpp"

namespace ProductCalculation
{
	class PCalcFactory;
}

BEGIN_PCALC_LIB_NAMESPACE

private ref class PCalcFactory
{
public:
	PCalcFactory(void);
	~PCalcFactory(void);
	ProductCalculation::IPCalcConfigurationPtr GetConfig(void);
	ProductCalculation::IPCalcManagerPtr GetPCalcMgr(void);
	ProductCalculation::IProductDescriptionPtr GetProdDesc(void);
	ProductCalculation::IPCalcActionManagerPtr GetActionMgr(void);
	ProductCalculation::IPTMgrPtr GetPTMgr(void);
	ProductCalculation::IPCalcMemoryKeyManagerPtr GetPCalcMemoryKeyMgr(void);
	ProductCalculation::IPCalcGeneralInfoManagerPtr GetPCalcGeneralInfoMgr(void);
	ProductCalculation::IPCalcReportManagerPtr GetPCalcReportMgr(void);

	property ProductCalculation::PCalcFactory* FactoryPtr
	{
		ProductCalculation::PCalcFactory* get()
		{
			return m_pFactory;
		}
	}

protected:
	!PCalcFactory(void);

private:
	ProductCalculation::PCalcFactory* m_pFactory;
};

END_PCALC_LIB_NAMESPACE



