#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

private ref class Lock
{
	public:
		Lock(System::Object^ syncLock)
			: m_SyncLock(syncLock)
		{
			System::Threading::Monitor::Enter(m_SyncLock);
		}

		~Lock()
		{
			System::Threading::Monitor::Exit(m_SyncLock);
		}

	private:
		System::Object^ m_SyncLock;
};

END_PCALC_LIB_NAMESPACE