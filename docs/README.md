# How To and Further Details - Unity Kolab Environment

# Contents

* [Application Setup: Step by Step](#Application-Setup:-Step-by-Step)
* [Interacting with the Kolab Environment](#Interacting-with-the-Kolab-Environment)
* [System Architecture Overview](#System-Architecture-Overview)
* [The Unity Implementation](#The-Unity-Implementation)
* [Unity/Renderer Interop Protocol](#Unity/Renderer-Interop-Protocol)
* [Getting VR-capable Renderer Projects](#Getting-VR-capable-Renderer-Projects)
* [Collaborative Mode via Photon](#Collaborative-Mode-via-Photon)
* [Issues and Future Work](#Issues-and-Future-Work)

# Application Setup: Step by Step

Getting the Kolab Environment up and running requires you to set up five key components of the system:
1. [Set up VR hardware and software](#1.-Setting-up-VR-Hardware-and-Software-on-Machine) on your machine. UnityKolabEnv requires **SteamVR** with a compatible **VR headset**, e.g. HTC VIVE (Pro).
2. [Run Unity Kolab Environment](#2.-Run-UnityKolabEnv) (this repository) in Unity 2019.1.1f1. Higher Unity versions may work but are not tested.
3. [Compile or download a SciVis Rendering Software](#3.-Get-VR-Compatible-SciVis-Renderers) that provides VR-Rendering components that connect to UnityKolabEnv, i.e. MegaMol or inviwo with their respective VR plugins/modules.
4. [Get Scientific Datasets and VR-compatible project-files](#4.-Scientific-Datasets-and-Renderer-VR-Projects) for your SciVis Renderers
5. [Set up the KolabLoader](#5.-Set-up-the-KolabLoader) with correct paths to the SciVis Renderers so UnityKolabEnv can find them

## 1. Setting up VR Hardware and Software on Machine

To properly set up your **VR headset** follow the instructions of the headset vendor. 
We tested UnityKolabEnv with the HTC VIVE and HTC VIVE Pro during development, but other VR headsets will probably also work as long as they are SteamVR-compatible. 
UnityKolabEnv uses the OpenVR API interface of Unity to talk to VR hardware. 
This means that you need SteamVR (see [steampowered.com/steamvr](https://store.steampowered.com/steamvr)) and compatible VR hardware to use UnityKolabEnv.

## 2. Run UnityKolabEnv

To run UnityKolabEnv you need Unity in version 2019.1.1f1. 
Higher Unity versions may work but are not tested.
You can run UnityKolabEnv by opening this repository as a Unity Project.
In the ``Assets`` directory you find the two main Scenes of the project: 
* The ``Workspaces`` scene contains UI elements and logic to start a collaborative remote session to which other users can connect via network
* The ``MintMain`` scene contains the implementation of the environment for scientific dataset analysis. When started, it will initiate an offline session where everything works as usual, but without remote collaboration

Note that UnityKolabEnv checks the congifuration file ``BaseSpoutInteropsettings.json`` in the user directory at ``AppData\LocalLow\UlmUniversity\``. This file is written and managed by the KolabLoader and UnityKolabEnv may not be fully functional until this file is configured properly. If UnityKolabEnv does not behave as expected, one source of error may be a wrong configuration in this file. 

You can also build a standalone executable of UnityKolabEnv using Unity (see [unity3d.com/Manual/PublishingBuilds](https://docs.unity3d.com/Manual/PublishingBuilds.html)). 
Make sure the build contains the scenes ``Workspaces`` and ``MintMain``.
Users can run the resulting standalone executable of UnityKolabEnv without an installation of Unity.

## 3. Get VR-Compatible SciVis Renderers

## 4. Scientific Datasets and Renderer VR-Projects

## 5. Set up the KolabLoader

# Interacting with the Kolab Environment

TODO

# System Architecture Overview

TODO

# Unity Implementation Overview

TODO

# Unity/Renderer Interop Protocol

TODO

# Getting VR-capable Renderer Projects

TODO

# Collaborative Mode via Photon

TODO

# Issues and Future Work

TODO link bugtrackers, github issues etc

Based on the year-1 review of the project, some specific areas are to be worked on in the future:
* Improving the system (robustness) and software packaging to a more production-ready state
* Making the VR part more generic to be usable for other project partners (e.g. tool abstractions, comm protocols)
* Switching to an own networking solution (instead of Photon), which HLRS has to develop
* Implementing and integrating with the Kolab "platform", for proper session / user handling and increased privacy and data security.

Beside those high level tasks, additional smaller tasks can be found in the Issue Tracker of this repo.

# TODO: how to produce executable application