#include "Convert.hpp"

#include <windows.h>
#include <string>

using namespace System;
using namespace System::Runtime::InteropServices;

System::String^ FP::Cloud::OnlineRateTable::PCalcLib::Convert::ToString(std::string source)
{
	System::IntPtr _ep = (IntPtr)(char*)source.c_str();
	String^ result = Marshal::PtrToStringAnsi(_ep);;
	return result;
}

System::String^ FP::Cloud::OnlineRateTable::PCalcLib::Convert::ToString(std::wstring source)
{
	System::IntPtr _ep = (IntPtr)(wchar_t*)source.data();
	String^ result = Marshal::PtrToStringUni(_ep);;
	return result;
}

std::string FP::Cloud::OnlineRateTable::PCalcLib::Convert::ToString(System::String^ source)
{
	std::string target;
	Convert::FromString(source, target);
	return target;
}

void FP::Cloud::OnlineRateTable::PCalcLib::Convert::FromString(System::String^ source, std::wstring &target)
{
	if (source)
	{
		System::IntPtr _ep = Marshal::StringToHGlobalUni(source);
		target = ((const wchar_t*)_ep.ToPointer());
		Marshal::FreeHGlobal(_ep);
	}
}

void FP::Cloud::OnlineRateTable::PCalcLib::Convert::FromString(System::String^ source, std::string &target)
{
	if (source)
	{
		System::IntPtr _ep = Marshal::StringToHGlobalAnsi(source);
		target = ((const char*)_ep.ToPointer());
		Marshal::FreeHGlobal(_ep);
	}
}
