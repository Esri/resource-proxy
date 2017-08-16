#pragma once

#include "boost/variant.hpp"
#include "PCalcLib.hpp"
#include "Convert.hpp"
#include "ProductCalculation/src/PlainProdTableDefs.hpp"

BEGIN_PCALC_LIB_NAMESPACE

class DisplayTypeVisitor : public boost::static_visitor<System::String^>
{
public:
	System::String^ operator()(std::wstring value) const
	{
		return Convert::ToString(value);
	}

	System::String^ operator()(PT::PTGraphicPtr pGraphic) const
	{
		return nullptr;
	}
};

END_PCALC_LIB_NAMESPACE

