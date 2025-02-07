/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID AMBIENCEBED = 881214142U;
        static const AkUniqueID CHASE_MUSIC = 3824731861U;
        static const AkUniqueID DOOR_CLOSING = 1591781701U;
        static const AkUniqueID DOOR_OPENING = 2589897592U;
        static const AkUniqueID FIRST_TIME_MONSTER = 3639762312U;
        static const AkUniqueID FIRST_TIME_STATION = 3937195198U;
        static const AkUniqueID LAST_CHASE_MUSIC = 2540848410U;
        static const AkUniqueID PLAYERFOOT_RUN = 4265836956U;
        static const AkUniqueID PLAYERFOOT_WALK = 744081578U;
        static const AkUniqueID ROOM_MUSIC = 1808177020U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace CHASED
        {
            static const AkUniqueID GROUP = 493461511U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID RUNNING = 3863236874U;
                static const AkUniqueID SEARCHING = 184992947U;
            } // namespace STATE
        } // namespace CHASED

        namespace LAST_CHASE
        {
            static const AkUniqueID GROUP = 423500810U;

            namespace STATE
            {
                static const AkUniqueID ALMOST_THERE = 833258532U;
                static const AkUniqueID MIDDLE = 1026627430U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID START = 1281810935U;
            } // namespace STATE
        } // namespace LAST_CHASE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace FOOTSTEPS_MATERIAL
        {
            static const AkUniqueID GROUP = 1682461626U;

            namespace SWITCH
            {
                static const AkUniqueID FILLED = 1735970877U;
                static const AkUniqueID HOLLOW = 815678804U;
            } // namespace SWITCH
        } // namespace FOOTSTEPS_MATERIAL

        namespace ROOMS
        {
            static const AkUniqueID GROUP = 1359360203U;

            namespace SWITCH
            {
                static const AkUniqueID ELECTRONICS_ROOM = 2572679272U;
                static const AkUniqueID HALLWAY = 2198133169U;
                static const AkUniqueID LABORATORY1 = 2864961111U;
                static const AkUniqueID LABORATORY2 = 2864961108U;
                static const AkUniqueID OBSERVATION_DECK = 3464770571U;
                static const AkUniqueID SECURITY_ROOM = 3965376459U;
            } // namespace SWITCH
        } // namespace ROOMS

    } // namespace SWITCHES

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID ENVIROMENT = 3909959462U;
        static const AkUniqueID MENUSOUNDS = 2805599468U;
        static const AkUniqueID MONSTERSOUNDS = 682291357U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID PLAYERSOUNDS = 1327972334U;
        static const AkUniqueID RIDDLES = 1872262486U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID SFX = 393239870U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID ELECTRONICS_ROOM = 2572679272U;
        static const AkUniqueID ESCAPE_POD = 1804753788U;
        static const AkUniqueID HALLWAYS = 4053971408U;
        static const AkUniqueID LABORATORY = 766371778U;
        static const AkUniqueID OBSERVATION_DECK = 3464770571U;
        static const AkUniqueID SECURITY_ROOM = 3965376459U;
        static const AkUniqueID STARTING_ROOM = 65866301U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
