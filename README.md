# Unity Kolab Environment - Collaborative Scientific Data Analysis in Virtual Reality

This repo contains the VR software part for the scientific visualization and collaboration environment created in the project Kolab-BW, funded by the MWK (state of Baden-Württemberg, Germany). It was created by the viscom group of Ulm University, Institute of Media Informatics (Prof. Ropinski) and VISUS Institute of Stuttgart University (Prof. Weiskopf).

# Contents

* [System Architecture](#system-architecture)
* [System Requirements](#system-requirements)
* [Quick Start](#quick-start)
* [System Features](#system-features)
* [Code Repositories](#code-repositories)
* [Future Work](#future-work)

# System Architecture

The overall Kolab system consists of the VR application ([this repo](https://github.com/UniStuttgart-VISUS/MWK-UnityKolabEnv)), a number of connected renderers (currently: [inviwo](https://inviwo.org/), [MegaMol](https://megamol.org/), internal renderer) and a [Loader application](https://github.com/UniStuttgart-VISUS/MWK-KolabLoader). As a networking infrastructure, [Photon](https://www.photonengine.com/) is being used right now, however it is planned to be replaced by an own solution created in year 2 of the Kolab project.

The renderers are running on the respective participating nodes locally. They communicate with the VR component through local ZeroMQ in a predefined message format (todo: separate page for comm protocol). The VR component broadcasts the renderer and player states through the Photon connection to other participants, thus keeping all participating instances in realtime sync. Additionally, voice communication channels (broadcast only for now) are being established through Photon as well.

# System Requirements

See [Code Repositories](#code-repositories) for corresponding source code.

* Unity 2019.1.1f1
* inviwo / MegaMol, compiled with Spout and ZeroMQ support
* Example Kolab VR projects and corresponding data (todo: link to data and project examples)
* Powerful PC (Testing environment: i7 9700K, RTX2080, 64GB RAM)
* VR Headset/HMD (tested with Vive and Vive Pro)
* Visual Studio (for Loader Development only)

# Quick Start

* Clone, CMake and Compile inviwo / MegaMol with ZeroMQ / Spout / Vrinterop plugins
* Clone this repo, import to Unity
* Clone / download example workspaces (todo: from where?)
* Clone Loader repo, compile
* Run loader, set initial config values (paths to EXEs, Workspace Name - has to be matching for all participants)
* Save config in Loader (JSON file is being saved to ```\AppData\LocalLow\UlmUniversity\BaseSpoutInteropSettings.json```)
* Run Unity **Workspaces** scene for collaborative session via network
* The **MintMain** scene contains the implementation of the environment

# System Features

* Basic VR movement (walking, teleport), including real-life collision avoidance for colocated multi-user VR scenarios
* Voice communication (right grip button, "push to talk"-style)
* Transparent handling of workspace files via loader panel in VR (point+click handles all necessary steps to fire up renderers, etc.)
* Dataset interactions: stick controller into the visualization - trigger right rotates, trigger left moves, both triggers zoom
* Engineeering dataset/internal renderer: exploded view (via pad up/down)
* Cutting Plane: virtual tool which can be grabbed and placed in the dataset to cut it
* Transfer function editor:
    * Point + trigger at TF rail: new point on the TF
    * Point, trigger hold and move left/right: move point (or min/max cone) on the TF
    * Point, trigger hold and move up: delete point on the TF
    * Point + trigger at TF swatch: open color chooser

# Code Repositories

Several repositories arised during development of the system. To give credit to the people involved and keep track of the developments made in those projects, the repositories are listed below.
Be aware that some of the respositories use Git LFS and Git Submodules. Thus, clone the repos with LFS installed and recursively clone submodules.

* Unity Kolab Environment / BaseSpoutInterop - the Unity Engine VR environment, collaborative labratory scene and integration of interop-communication protocols. Probably only usable on Windows.
    * Main: [UniStuttgart-VISUS/MWK-UnityKolabEnv](https://github.com/UniStuttgart-VISUS/MWK-UnityKolabEnv)
    * Initial concept and prototype (phase 1): [fg-uulm/BaseSpoutInterop](https://github.com/fg-uulm/BaseSpoutInterop)
    * Fork for phase 2: [geringsj/BaseSpoutInterop](https://github.com/geringsj/BaseSpoutInterop)
    * Prototyping for phase 1: [UniStuttgart-VISUS/MWK-UnityScivisInteropVR](https://github.com/UniStuttgart-VISUS/MWK-UnityScivisInteropVR), [geringsj/MWK-UnityScivisInteropVR](https://github.com/geringsj/MWK-UnityScivisInteropVR)
* Kolab Loader - Configuration and Startup Manager in C# for Windows
    * Main: [UniStuttgart-VISUS/MWK-KolabLoader](https://github.com/UniStuttgart-VISUS/MWK-KolabLoader)
    * Concept: [fg-uulm/KolabLoader](https://github.com/fg-uulm/KolabLoader)
* Minimal Interop Library - Prototyping of Communication Protocols and Integration of ZeroMQ (interprocess communication) and Spout2 (interprocess texture sharing) into one C++ Library
    * Main: [UniStuttgart-VISUS/MWK-mint](https://github.com/UniStuttgart-VISUS/MWK-mint)
    * Concept: [geringsj/MWK-mint](https://github.com/geringsj/MWK-mint)
        * Spout2 with necessary fixes: use-after-free-fix branch at [geringsj/Spout2](https://github.com/geringsj/Spout2)
        * ZeroMQ with necessary fixes: cmake_fix branch at [geringsj/cppzmq](https://github.com/geringsj/cppzmq), master at [geringsj/libzmq](https://github.com/geringsj/libzmq)
        * Json from [nlohmann/json](https://github.com/nlohmann/json) is used
* Repositories/Branches of Scientific Visualisation Software implementing the communication protocol for UnityKolabEnv
    * inviwo - [https://inviwo.org/](https://inviwo.org/)
        * Main: mint branch and spout/zmq modules at [geringsj/inviwo](https://github.com/geringsj/inviwo)
        * Concept at [Sparkier/inviwo](https://github.com/Sparkier/inviwo)
    * MegaMol - [https://megamol.org/](https://megamol.org/)
        * Main: vrinterop branch and plugin at [UniStuttgart-VISUS/megamol](https://github.com/UniStuttgart-VISUS/megamol)
        * Concept, vrinterop branch and plugin at [geringsj/megamol](https://github.com/geringsj/megamol)

# Future Work

Based on the year-1 review of the project, some specific areas are to be worked on in the future:
* Improving the system (robustness) and software packaging to a more production-ready state
* Making the VR part more generic to be usable for other project partners (e.g. tool abstractions, comm protocols)
* Switching to an own networking solution (instead of Photon), which HLRS has to develop
* Implementing and integrating with the Kolab "platform", for proper session / user handling and increased privacy and data security.

Beside those high level tasks, additional smaller tasks can be found in the Issue Tracker of this repo.