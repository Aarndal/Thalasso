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
        static const AkUniqueID CHASE_MUSIC = 3824731861U;
        static const AkUniqueID CIRCUITRIDDLE_CLICK = 1065298177U;
        static const AkUniqueID CIRCUITRIDDLE_COMPLETE = 2222894834U;
        static const AkUniqueID CIRCUITRIDDLE_RIGHT = 4250834603U;
        static const AkUniqueID CIRCUITRIDDLE_WRONG = 1404039910U;
        static const AkUniqueID DOOROPEN = 1404805401U;
        static const AkUniqueID ELECTRONICSROOM_HUM = 898570444U;
        static const AkUniqueID END_CREDITS = 3797583859U;
        static const AkUniqueID ESCAPEPODDOOROPEN = 576245037U;
        static const AkUniqueID LAST_CHASE_MUSIC = 2540848410U;
        static const AkUniqueID MAIN_MENU_IN = 580468006U;
        static const AkUniqueID MAIN_MENU_MUSIC = 3399698792U;
        static const AkUniqueID MAIN_MENU_OUT = 3091888041U;
        static const AkUniqueID MASTERMINDBUTTONPRESS = 847120340U;
        static const AkUniqueID MASTERMINDCORRECT = 3511230007U;
        static const AkUniqueID MASTERMINDWRONG = 2240061350U;
        static const AkUniqueID MENU_BACK_BUTTON = 2351299877U;
        static const AkUniqueID MENU_BUTTON_PRESS = 952003163U;
        static const AkUniqueID MUSIC_AMBIENCE_FIRST_TIME_MONSTER = 1832321273U;
        static const AkUniqueID MUSIC_AMBIENCE_FIRST_TIME_STATION = 3531297571U;
        static const AkUniqueID PAUSE_MENU_IN = 3382955325U;
        static const AkUniqueID PAUSE_MENU_OUT = 2397763428U;
        static const AkUniqueID PLAY_AUSTIN = 3408260292U;
        static const AkUniqueID PLAYAMBIENCELOOP = 206142921U;
        static const AkUniqueID PLAYAMBIENCEONESHOTS = 1968327068U;
        static const AkUniqueID PLAYERFOOT_RUN = 4265836956U;
        static const AkUniqueID PLAYERFOOT_WALK = 744081578U;
        static const AkUniqueID ROOM_MUSIC = 1808177020U;
        static const AkUniqueID SMALLDOORCLOSECLICK = 3301454522U;
        static const AkUniqueID SMALLDOORCLOSECREAK = 779185414U;
        static const AkUniqueID SMALLDOOROPENCLICK = 1277509966U;
        static const AkUniqueID SMALLDOOROPENCREAK = 488029002U;
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
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID STEEL = 1366287314U;
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

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID BGM = 412724365U;
        static const AkUniqueID MASTER = 4056684167U;
        static const AkUniqueID SFX = 393239870U;
    } // namespace GAME_PARAMETERS

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
        static const AkUniqueID LABORATORY1 = 2864961111U;
        static const AkUniqueID LABORATORY2 = 2864961108U;
        static const AkUniqueID OBSERVATION_DECK = 3464770571U;
        static const AkUniqueID OUTSIDE_SECURITY_ROOM = 2844374603U;
        static const AkUniqueID OUTSIDE_STARTING_ROOM = 3239831741U;
        static const AkUniqueID SECURITY_ROOM = 3965376459U;
        static const AkUniqueID SMALL_OFFICE = 3078522397U;
        static const AkUniqueID STARTING_ROOM = 65866301U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
