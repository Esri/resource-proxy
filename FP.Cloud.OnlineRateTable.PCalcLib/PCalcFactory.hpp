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
	PT::IPCalcConfigurationPtr GetConfig(void);
	PT::IPCalcManagerPtr GetPCalcMgr(void);
	PT::IProductDescriptionPtr GetProdDesc(void);
	PT::IPCalcActionManagerPtr GetActionMgr(void);
	PT::IPTMgrPtr GetPTMgr(void);
	PT::IPCalcMemoryKeyManagerPtr GetPCalcMemoryKeyMgr(void);
	PT::IPCalcGeneralInfoManagerPtr GetPCalcGeneralInfoMgr(void);
	PT::IPCalcReportManagerPtr GetPCalcReportMgr(void);

	property PT::PCalcFactory* FactoryPtr
	{
		PT::PCalcFactory* get()
		{
			return m_pFactory;
		}
	}

protected:
	!PCalcFactory(void);

private:
	PT::PCalcFactory* m_pFactory;
};

END_PCALC_LIB_NAMESPACE



