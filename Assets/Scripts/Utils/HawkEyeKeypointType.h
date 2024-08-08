/*
* "Software is a process, it's never finished, it's always evolving.
* That's its nature. We know our software sucks. But it's shipping!
* Next time we'll do better, but even then it will be shitty.
* The only software that's perfect is one you're dreaming about.
* Real software crashes, loses data, is hard to learn and hard to use.
* But it's a process. We'll make it less shitty. Just watch!"
*/

#if !defined(HAWKEYE_KEYPOINT_TYPE_INCLUDED)
#define HAWKEYE_KEYPOINT_TYPE_INCLUDED

#include <map>

#include <boost/algorithm/string.hpp>

#include <Common.h>
#include <KeypointType.h>

enum HAWKEYE_KEYPOINT_TYPE {
    NOSE_HAWKEYE_KEYPOINT = 0,
    NECK_HAWKEYE_KEYPOINT = 1,
    RIGHT_SHOULDER_HAWKEYE_KEYPOINT = 2,
    RIGHT_ELBOW_HAWKEYE_KEYPOINT = 3,
    RIGHT_WRIST_HAWKEYE_KEYPOINT = 4,
    LEFT_SHOULDER_HAWKEYE_KEYPOINT = 5,
    LEFT_ELBOW_HAWKEYE_KEYPOINT = 6,
    LEFT_WRIST_HAWKEYE_KEYPOINT = 7,
    MIDDLE_HIP_HAWKEYE_KEYPOINT = 8,
    RIGHT_HIP_HAWKEYE_KEYPOINT = 9,
    RIGHT_KNEE_HAWKEYE_KEYPOINT = 10,
    RIGHT_ANKLE_HAWKEYE_KEYPOINT = 11,
    LEFT_HIP_HAWKEYE_KEYPOINT = 12,
    LEFT_KNEE_HAWKEYE_KEYPOINT = 13,
    LEFT_ANKLE_HAWKEYE_KEYPOINT = 14,
    RIGHT_EYE_HAWKEYE_KEYPOINT = 15,
    LEFT_EYE_HAWKEYE_KEYPOINT = 16,
    RIGHT_EAR_HAWKEYE_KEYPOINT = 17,
    LEFT_EAR_HAWKEYE_KEYPOINT = 18,
    LEFT_BIG_TOE_HAWKEYE_KEYPOINT = 19,
    LEFT_SMALL_TOE_HAWKEYE_KEYPOINT = 20,
    LEFT_HEEL_HAWKEYE_KEYPOINT = 21,
    RIGHT_BIG_TOE_HAWKEYE_KEYPOINT = 22,
    RIGHT_SMALL_TOE_HAWKEYE_KEYPOINT = 23,
    RIGHT_HEEL_HAWKEYE_KEYPOINT = 24,
    LAST_HAWKEYE_KEYPOINT_TYPE = 25
};

/**
*/
inline std::string GetHawkEyeKeypointTypeString(int hawkEyeKeypointType)
{
    std::string hawkEyeKeypointTypeString[] = {
        "Nose",
        "Neck",
        "RShoulder",
        "RElbow",
        "RWrist",
        "LShoulder",
        "LElbow",
        "LWrist",
        "MidHip",
        "RHip",
        "RKnee",
        "RAnkle",
        "LHip",
        "LKnee",
        "LAnkle",
        "REye",
        "LEye",
        "REar",
        "LEar",
        "LBigToe",
        "LSmallToe",
        "LHeel",
        "RBigToe",
        "RSmallToe",
        "RHeel",
        "Unknown"
    };

    if (hawkEyeKeypointType >= LAST_HAWKEYE_KEYPOINT_TYPE)
        return my::Null<std::string>();

    return hawkEyeKeypointTypeString[hawkEyeKeypointType];
}

/**
*/
inline HAWKEYE_KEYPOINT_TYPE GetHawkEyeKeypointTypeFromInteger(my::int32 hawkEyeKeypointType)
{
    HAWKEYE_KEYPOINT_TYPE hawkEyeKeypointTypeArray[] = {
        NOSE_HAWKEYE_KEYPOINT,
        NECK_HAWKEYE_KEYPOINT,
        RIGHT_SHOULDER_HAWKEYE_KEYPOINT,
        RIGHT_ELBOW_HAWKEYE_KEYPOINT,
        RIGHT_WRIST_HAWKEYE_KEYPOINT,
        LEFT_SHOULDER_HAWKEYE_KEYPOINT,
        LEFT_ELBOW_HAWKEYE_KEYPOINT,
        LEFT_WRIST_HAWKEYE_KEYPOINT,
        MIDDLE_HIP_HAWKEYE_KEYPOINT,
        RIGHT_HIP_HAWKEYE_KEYPOINT,
        RIGHT_KNEE_HAWKEYE_KEYPOINT,
        RIGHT_ANKLE_HAWKEYE_KEYPOINT,
        LEFT_HIP_HAWKEYE_KEYPOINT,
        LEFT_KNEE_HAWKEYE_KEYPOINT,
        LEFT_ANKLE_HAWKEYE_KEYPOINT,
        RIGHT_EYE_HAWKEYE_KEYPOINT,
        LEFT_EYE_HAWKEYE_KEYPOINT,
        RIGHT_EAR_HAWKEYE_KEYPOINT,
        LEFT_EAR_HAWKEYE_KEYPOINT,
        LEFT_BIG_TOE_HAWKEYE_KEYPOINT,
        LEFT_SMALL_TOE_HAWKEYE_KEYPOINT,
        LEFT_HEEL_HAWKEYE_KEYPOINT,
        RIGHT_BIG_TOE_HAWKEYE_KEYPOINT,
        RIGHT_SMALL_TOE_HAWKEYE_KEYPOINT,
        RIGHT_HEEL_HAWKEYE_KEYPOINT,
        LAST_HAWKEYE_KEYPOINT_TYPE
    };

    if (hawkEyeKeypointType >= LAST_HAWKEYE_KEYPOINT_TYPE)
        return LAST_HAWKEYE_KEYPOINT_TYPE;

    return hawkEyeKeypointTypeArray[hawkEyeKeypointType];
}

/**
*/
inline my::int32 GetKeypointTypeFromHawkEyeInteger(my::int32 hawkEyeKeypointType)
{
    std::map<my::int32, my::int32> hawkEyeKeypointTypeToKeypointTypeMap = {
        { NOSE_HAWKEYE_KEYPOINT, my::NOSE_KEYPOINT },
        { NECK_HAWKEYE_KEYPOINT, my::NECK_KEYPOINT },
        { RIGHT_SHOULDER_HAWKEYE_KEYPOINT, my::RIGHT_SHOULDER_KEYPOINT },
        { RIGHT_ELBOW_HAWKEYE_KEYPOINT, my::RIGHT_ELBOW_KEYPOINT },
        { RIGHT_WRIST_HAWKEYE_KEYPOINT, my::RIGHT_WRIST_KEYPOINT },
        { LEFT_SHOULDER_HAWKEYE_KEYPOINT, my::LEFT_SHOULDER_KEYPOINT },
        { LEFT_ELBOW_HAWKEYE_KEYPOINT, my::LEFT_ELBOW_KEYPOINT },
        { LEFT_WRIST_HAWKEYE_KEYPOINT, my::LEFT_WRIST_KEYPOINT },
        { RIGHT_HIP_HAWKEYE_KEYPOINT, my::RIGHT_HIP_KEYPOINT },
        { RIGHT_KNEE_HAWKEYE_KEYPOINT, my::RIGHT_KNEE_KEYPOINT },
        { RIGHT_ANKLE_HAWKEYE_KEYPOINT, my::RIGHT_ANKLE_KEYPOINT },
        { LEFT_HIP_HAWKEYE_KEYPOINT, my::LEFT_HIP_KEYPOINT },
        { LEFT_KNEE_HAWKEYE_KEYPOINT, my::LEFT_KNEE_KEYPOINT },
        { LEFT_ANKLE_HAWKEYE_KEYPOINT, my::LEFT_ANKLE_KEYPOINT },
        { RIGHT_EYE_HAWKEYE_KEYPOINT, my::RIGHT_EYE_KEYPOINT },
        { LEFT_EYE_HAWKEYE_KEYPOINT, my::LEFT_EYE_KEYPOINT },
        { RIGHT_EAR_HAWKEYE_KEYPOINT, my::RIGHT_EAR_KEYPOINT },
        { LEFT_EAR_HAWKEYE_KEYPOINT, my::LEFT_EAR_KEYPOINT }
    };

    std::map<my::int32, my::int32>::const_iterator hawkEyeKeypointTypeToKeypointTypeIterator = hawkEyeKeypointTypeToKeypointTypeMap.find(hawkEyeKeypointType);

    if (hawkEyeKeypointTypeToKeypointTypeIterator != hawkEyeKeypointTypeToKeypointTypeMap.end())
        return hawkEyeKeypointTypeToKeypointTypeIterator->second;

    return my::Null<my::int32>();
}

/**
*/
inline HAWKEYE_KEYPOINT_TYPE GetHawkEyeKeypointTypeFromString(std::string hawkEyeKeypointTypeString)
{
    boost::trim(hawkEyeKeypointTypeString);

    if (hawkEyeKeypointTypeString == "Nose")
        return NOSE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "Neck")
        return NECK_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "RShoulder")
        return RIGHT_SHOULDER_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "RElbow")
        return RIGHT_ELBOW_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "RWrist")
        return RIGHT_WRIST_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LShoulder")
        return LEFT_SHOULDER_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LElbow")
        return LEFT_ELBOW_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LWrist")
        return LEFT_WRIST_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "MidHip")
        return  MIDDLE_HIP_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "RHip")
        return RIGHT_HIP_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "RKnee")
        return RIGHT_KNEE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "RAnkle")
        return RIGHT_ANKLE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LHip")
        return LEFT_HIP_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LKnee")
        return LEFT_KNEE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LAnkle")
        return LEFT_ANKLE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "REye")
        return RIGHT_EYE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LEye")
        return LEFT_EYE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "REar")
        return RIGHT_EAR_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LEar")
        return LEFT_EAR_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LBigToe")
        return LEFT_BIG_TOE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LSmallToe")
        return LEFT_SMALL_TOE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "LHeel")
        return LEFT_HEEL_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "RBigToe")
        return RIGHT_BIG_TOE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "RSmallToe")
        return RIGHT_SMALL_TOE_HAWKEYE_KEYPOINT;
    if (hawkEyeKeypointTypeString == "RHeel")
        return RIGHT_HEEL_HAWKEYE_KEYPOINT;

    return LAST_HAWKEYE_KEYPOINT_TYPE;
}

#endif // #if !defined(HAWKEYE_KEYPOINT_TYPE_INCLUDED)

