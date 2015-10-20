#include <string>
#include <vector>

#include "Base/misc/std_def.h"
#include "Base/misc/ConvertType.hpp"
#include "ProductCalculation/Windows/PCalcSimulation/PCalcManagedLib/PCalcManagedLib.h"

#include "boost/cast.hpp"

using namespace System::Collections::Generic;
using namespace System;

namespace VariantSpecific
{
	public ref class MyZip2ZoneConverter abstract
	{
	public:
		static void SetInstance(MyZip2ZoneConverter^ value)
		{
			m_Instance = value;
		}

		static MyZip2ZoneConverter^ GetInstance()
		{
			return m_Instance;
		}

		virtual IEnumerable<INT32>^ GetZones(String^ currentZipCode, String^ targetZipCode) = 0;

	private:
		static MyZip2ZoneConverter^ m_Instance;
	};

	bool MapZip2Zone(const std::string& currentLocation, const std::string& targetLocation, std::vector<INT32>& zoneList)
	{
		String^ currentZipCode = PCalcManagedLib::ConvertToNetString(currentLocation);
		String^ targetZipCode = PCalcManagedLib::ConvertToNetString(targetLocation);

		MyZip2ZoneConverter^ converter = MyZip2ZoneConverter::GetInstance();
		if (nullptr == converter)
		{
			return false;
		}

		IEnumerable<INT32>^ result;
		try
		{
			result = converter->GetZones(currentZipCode, targetZipCode);
		}
		catch (Exception^)
		{
			return false;
		}

		zoneList.clear();
		for each(INT32 current in result)
		{
			zoneList.push_back(current);
		}
		return true;
	}
}