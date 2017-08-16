#pragma once

#include "Base/misc/assert_m.h"
#include <string>
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

private ref class Convert
{
public:
	static System::String^ ToString(std::string source);
	static System::String^ ToString(std::wstring source);
	static std::string ToString(System::String^ source);

	static void FromString(System::String^ source, std::wstring &target);
	static void FromString(System::String^ source, std::string &target);
};

END_PCALC_LIB_NAMESPACE
