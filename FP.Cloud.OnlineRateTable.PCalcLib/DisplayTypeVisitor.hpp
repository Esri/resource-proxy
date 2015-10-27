#pragma once

#include "boost/variant.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

class DisplayTypeVisitor : public boost::static_visitor<System::String^>
{
public:
	System::String^ operator()(std::wstring value) const
	{
		return PCalcManagedLib::ConvertWStringToNetString(value);
	}

	System::String^ operator()(ProductCalculation::PTGraphicPtr pGraphic) const
	{
		return nullptr;
	}
};

END_PCALC_LIB_NAMESPACE

