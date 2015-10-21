#pragma once

#include "ProductCalculation/IPCalcManager.hpp"
#include "ProductCalculation/IPCalcConfiguration.hpp"
#include "ProductCalculation/IProductDescription.hpp"
#include "ProductCalculation/IPCalcActionManager.hpp"
#include "ProductCalculation/IPTMgr.hpp"
#include "ProductCalculation/IPCalcMemoryKeyManager.hpp"
#include "ProductCalculation/IPCalcGeneralInfoManager.hpp"
#include "ProductCalculation/IPCalcErrorLogManager.hpp"
#include "ProductCalculation/IPCalcReportManager.hpp"
#include "PCalcLib.hpp"

namespace ProductCalculation {
	class PCalcManager;
}

namespace ProductCalculation {

	class PCalcFactory
	{
	public:
		PCalcFactory();
		virtual ~PCalcFactory();
		virtual ProductCalculation::IPCalcManagerPtr GetPCalcMgr() const;
		virtual ProductCalculation::IPCalcConfigurationPtr GetPCalcConfig() const;
		virtual ProductCalculation::IPCalcActionManagerPtr GetPCalcActionMgr() const;
		virtual ProductCalculation::IProductDescriptionPtr GetProdDesc() const;
		virtual ProductCalculation::IPCalcMemoryKeyManagerPtr GetPCalcMemoryKeyMgr() const;
		virtual ProductCalculation::IPCalcGeneralInfoManagerPtr GetPCalcGeneralInfoMgr() const;
		virtual ProductCalculation::IPTMgrPtr GetPTMgr() const;
		virtual ProductCalculation::IPCalcErrorLogManagerPtr GetPCalcErrorLogMgr() const;
		virtual ProductCalculation::IPCalcReportManagerPtr GetPCalcReportMgr() const;

	private:
		boost::shared_ptr<ProductCalculation::PCalcManager> m_pPCalcMgr;
		PCalcFactory(const PCalcFactory &right);
		PCalcFactory & operator=(const PCalcFactory &right);

	};

}

