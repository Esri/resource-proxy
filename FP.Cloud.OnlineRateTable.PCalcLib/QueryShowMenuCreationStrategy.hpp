#pragma once

#include "QueryCreationStrategy.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class QueryShowMenuCreationStrategy : public QueryCreationStrategy
{
public:
	QueryShowMenuCreationStrategy(PCalcFactory^ factory);
	virtual void SetQueryDescription(PCalcResultInfo^ resultInfo) override;
};

END_PCALC_LIB_NAMESPACE




