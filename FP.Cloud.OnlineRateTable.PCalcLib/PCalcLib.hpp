#pragma once

#include "Base/misc/assert_m.h"

#define BEGIN_PCALC_LIB_NAMESPACE namespace FP { namespace Cloud { namespace OnlineRateTable { namespace PCalcLib {

#define END_PCALC_LIB_NAMESPACE  } } } }

#define USING_PCALC_LIB_NAMESPACE using namespace FP::Cloud::OnlineRateTable::PCalcLib;

#define USING_PRODUCTCALCULATION_NAMESPACE using namespace ProductCalculation; using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

namespace FP
{
	namespace Cloud
	{
		namespace OnlineRateTable
		{
			namespace PCalcLib {}
			namespace ProductCalculation {}
		}
	}
}

namespace ProductCalculation
{
}

namespace PT = ProductCalculation;
namespace Lib = FP::Cloud::OnlineRateTable::PCalcLib;
namespace Shared = FP::Cloud::OnlineRateTable::Common::ProductCalculation;
