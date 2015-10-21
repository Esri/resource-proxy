#pragma once

#include "PCalcFactory.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class QueryCreationStrategy abstract
{
public:
	void SetResult(PCalcResultInfo^ resultInfo);


protected:
	QueryCreationStrategy(PCalcFactory^ factory);
	virtual void SetQueryDescription(PCalcResultInfo^ resultInfo) abstract;

	property PCalcFactory^ Factory
	{
		PCalcFactory^ get() { return m_Factory; }
	}

private:
	PCalcFactory^ m_Factory;
};


END_PCALC_LIB_NAMESPACE