# LastManStanding
서버를 통한 멀티 게임 제작을 해보기 위해 작은 게임 하나를 제작하였습니다.
## 👨‍🏫 프로젝트 소개
"LastManStanding게임 방식과 같으며, 주어진 맵 내에서 AI와 플레이어들 중 상대 플레이어를 찾아내거나 정해진 여러 위치에 먼저 도달하는 플레이어가 승리하는 게임입니다.

## ⏲️ 개발 기간 
- 2024.06.16(목) ~ 2024.07.14(토)
- Photon을 통한 서버 구현
- Photon의 RPC를 통한 채팅 구현
- AI의 움직임 구현
  
## 🧑‍🤝‍🧑 개발자 소개 
- **김준우** : 게임 개발자

## 💻 개발환경
- **Version** : Unity(2022.03.21f1)
- **IDE** : Visual Studio IDE

## ⚙️ 기술 스택
- **Server** : Photon

## 📌 주요 기능
- 서버 구현
  - Unity서버를 열기 위해 Photon앱을 사용하여 게임 서버를 만들어내었습니다.
  - 플레이어는 게임 내에서 보여질 이름을 원하는대로 입력할 수 있습니다.
  - Host로 방을 생성하여 사람들이 들어올 수 있도록 하거나, 친구들끼리만 플레이가 가능하도록 PrivateRoom까지 제작하였습니다.
- 채팅 구현
   - 게임 내에서 소통이 가능하도록 채팅을 구현하였습니다.
   - Photon에서 주어진 RPC를 사용하여 채팅을 구현하였습니다.
   - 채팅 중에는 플레이어의 모든 입력을 막아두었습니다.
- AI 움직임 구현
    - Enum을 사용하여 IDLE, MOVE, DEAD로 나누어 각 상황에 알맞은 행동을 하도록 제작하였습니다.
    - NavMesh를 사용해 움직임을 제작하였으며, 일정 거리 이상 움직일 수 있도록 insideUnitSphere로 랜덤한 범위 내 위치를 찾고 이동하도록 구현하였습니다.

