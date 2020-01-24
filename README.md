# Overview

This repo contains the VR software part for the scientific visualization and collaboration environment created in the project Kolab-BW, funded by the MWK (state of Baden-Württemberg, Germany). It was created by the viscom group of Ulm University, Institute of Media Informatics (Prof. Ropinski) and visus Institute of Stuttgart University (Prof. Weiskopf).

# Contents

* [System Architecture](#system-architecture)
* [System Requirements](#system-requirements)
* [Quick Start](#quick-start)
* [Future Work](#future-work)

# System Architecture

The overall Kolab system consists of the VR application (this repo), a number of connected renderers (currently: [inviwo](https://inviwo.org/), [MegaMol](https://megamol.org/), internal renderer) and a Loader application (todo: insert github link). As a networking infrastructure, [Photon](https://www.photonengine.com/) is being used right now, however it is planned to be replaced by an own solution created in year 2 of the Kolab project.

The renderers are running on the respective participating nodes locally. They communicate with the VR component through local ZeroMQ in a predefined message format (todo: separate page for comm protocol). The VR component broadcasts the renderer and player states through the Photon connection to other participants, thus keeping all participating instances in realtime sync. Additionally, voice communication channels (broadcast only for now) are being established through Photon as well.

# System Requirements

* Unity 2019.1.1f1
* inviwo / Megamol, compiled with Spout and ZeroMQ support (todo: make local feature branches of sparkier and geringsj available somewhere and link them here)
* VR HMD (tested with Vive and Vive Pro)
* Powerful PC (Testing environment: i7 9700K, RTX2080, 64GB RAM)
* Visual Studio (for Loader Development only)

# Quick Start

* Clone, CMake and Compile inviwo / Megamol
* Clone this repo, import to Unity
* Clone / download example workspaces (todo: from where?)
* Clone Loader repo, compile
* Run loader, set initial config values (paths to EXEs, Workspace Name - has to be matching for all participants)
* Save config in Loader (JSON file is being saved to ```\AppData\LocalLow\UlmUniversity\BaseSpoutInteropSettings.json```)
* Run Unity scene

# System Features

* Basic VR movement (walking, teleport), including real-life collision avoidance for colocated multi-user VR scenarios
* Voice communication (right grip button, "push to talk"-style)
* Transparent handling of workspace files via loader panel in VR (point+click handles all necessary steps to fire up renderers, etc.)
* Dataset interactions: stick controller into the visualization - trigger right rotates, trigger left moves, both triggers zoom
* Engineeering dataset/internal renderer: exploded view (via pad up/down)
* Cutting Plane: virtual tool which can be grabbed and placed in the dataset to cut it
* Transfer function editor:
** Point + trigger at TF rail: new point on the TF
** Point, trigger hold and move left/right: move point (or min/max cone) on the TF
** Point, trigger hold and move up: delete point on the TF
** Point + trigger at TF swatch: open color chooser

# Future Work

Based on the year-1 review of the project, some specific areas are to be worked on in the future:
* Improving the system (robustness) and software packaging to a more production-ready state
* Making the VR part more generic to be usable for other project partners (e.g. tool abstractions, comm protocols)
* Switching to an own networking solution (instead of Photon), which HLRS has to develop
* Implementing and integrating with the Kolab "platform", for proper session / user handling and increased privacy and data security.

Beside those high level tasks, additional smaller tasks can be found in the Issue Tracker of this repo.