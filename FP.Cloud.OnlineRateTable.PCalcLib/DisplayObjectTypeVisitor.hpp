#pragma once

#include "PCalcLib.hpp"
#include "boost/variant.hpp"
#include <iosfwd>
#include "ProductCalculation/ProductDescriptionDefs.hpp"
#include "DisplayTypeVisitor.hpp"
#include "PCalcFactoryCPP.hpp"
#include "Convert.hpp"

BEGIN_PCALC_LIB_NAMESPACE

class DisplayObjectTypeVisitor : public boost::static_visitor<System::String^>
{
public:
	DisplayObjectTypeVisitor(ProductCalculation::PCalcFactory *factory)
		: m_Factory(factory)
	{
	}

	System::String^ operator()(INT32 value) const
	{
		std::wostringstream stream;
		stream << value;
		return Convert::ToString(stream.str());
	}

	System::String^ operator()(std::wstring value) const
	{
		return Convert::ToString(value);
	}

	System::String^ operator()(ProductCalculation::TextGraphicIdType value) const
	{
		ProductCalculation::IPTMgrPtr pPTMgr(m_Factory->GetPTMgr());
		ProductCalculation::DisplayType displayType(pPTMgr->GetDisplayObject(value.IsAscii, value.ID));
		return boost::apply_visitor(DisplayTypeVisitor(), displayType);
	}

private:
	ProductCalculation::PCalcFactory *m_Factory;
};

END_PCALC_LIB_NAMESPACE