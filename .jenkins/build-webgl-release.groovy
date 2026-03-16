// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

pipeline {
    agent any

    parameters {
        string(
            name: 'UNITY_EXECUTABLE',
            defaultValue: '/opt/unity/Editor/Unity',
            description: 'Unity 実行ファイルのフルパス。\nLinux 例: /opt/unity/Editor/Unity\nWindows 例: C:\\Program Files\\Unity\\Hub\\Editor\\6000.3.10f1\\Editor\\Unity.exe'
        )
    }

    options {
        timestamps()
        timeout(time: 60, unit: 'MINUTES')
    }

    stages {
        // ----------------------------------------------------------------
        // Stage 1: Addressables ビルド
        //   このプロジェクトは m_BuildAddressablesWithPlayerBuild が 0（無効）に
        //   設定されているため、Player ビルド時に Addressables が自動ビルドされません。
        //   WebGL ビルドの前に必ずここで Addressables をビルドします。
        // ----------------------------------------------------------------
        stage('Addressables ビルド') {
            steps {
                script {
                    runUnity(
                        executable: params.UNITY_EXECUTABLE,
                        logFile:    'Logs/ci-addressables.log',
                        extraArgs:  '-executeMethod Murnana.UnityRoom.Editor.CI.CIBuildScript.BuildAddressables'
                    )
                }
            }
            post {
                always {
                    // 成功・失敗にかかわらずログをアーカイブして、あとで確認できるようにする
                    archiveArtifacts(
                        artifacts:         'Logs/ci-addressables.log',
                        allowEmptyArchive: true
                    )
                }
            }
        }

        // ----------------------------------------------------------------
        // Stage 2: WebGL リリースビルド
        //   -buildProfile で BuildProfile アセットを指定して WebGL ビルドを実行します。
        //   Unity 6 以降は BuildProfile アセットに Player Settings が含まれるため、
        //   解像度・圧縮形式・Development Build の設定がプロファイルから自動適用されます。
        // ----------------------------------------------------------------
        stage('WebGL リリースビルド') {
            steps {
                script {
                    runUnity(
                        executable: params.UNITY_EXECUTABLE,
                        logFile:    'Logs/ci-webgl-release.log',
                        extraArgs:  '-buildProfile "Assets/Settings/Build Profiles/Web - Release.asset"'
                    )
                }
            }
            post {
                always {
                    archiveArtifacts(
                        artifacts:         'Logs/ci-webgl-release.log',
                        allowEmptyArchive: true
                    )
                }
            }
        }

        // ----------------------------------------------------------------
        // Stage 3: 成果物アーカイブ
        //   Builds/Release/ 以下の WebGL ビルド成果物を Jenkins にアーカイブします。
        //   unityroom への投稿に必要な以下 4 ファイルが含まれます:
        //     - ローダーファイル (.js)
        //     - データファイル (.data.gz)
        //     - フレームワークファイル (.framework.js.gz)
        //     - コードファイル (.wasm.gz)
        // ----------------------------------------------------------------
        stage('成果物アーカイブ') {
            steps {
                archiveArtifacts(
                    artifacts:         'Builds/Release/**',
                    allowEmptyArchive: false
                )
            }
        }
    }

    post {
        failure {
            script {
                // ビルドが失敗したとき、ログの末尾をコンソールに出力して
                // Jenkins の画面からすばやく原因を確認できるようにする
                String[] logFiles = [
                    'Logs/ci-addressables.log',
                    'Logs/ci-webgl-release.log'
                ]
                for (String logFile in logFiles) {
                    if (fileExists(logFile)) {
                        echo "=== ${logFile} (末尾 50 行) ==="
                        if (isUnix()) {
                            sh "tail -n 50 \"${logFile}\""
                        } else {
                            bat "powershell -Command \"Get-Content '${logFile}' -Tail 50\""
                        }
                    }
                }
            }
        }
        cleanup {
            // 成功・失敗・中断にかかわらず常にワークスペースを削除する。
            // Addressables の中間ファイルが次回ビルドに混入しないよう、
            // 毎回クリーンな状態から始めることを保証する。
            cleanWs()
        }
    }
}

// ----------------------------------------------------------------
// ヘルパー関数: Unity をバッチモードで起動する
//
// isUnix() で実行環境の OS を判定し、Linux/macOS では sh、
// Windows では bat を自動的に選択します。
// Unity はビルド成功時に終了コード 0、失敗時に終了コード 1 を返すため、
// Jenkins の sh/bat が自動的にステージを失敗状態にします。
// ----------------------------------------------------------------
def runUnity(Map args) {
    String projectPath = pwd()
    // 共通の Unity バッチモードフラグ:
    //   -batchmode  : GUI を表示せずにバックグラウンドで起動する
    //   -nographics : GPU レンダリングを無効にする（サーバー環境向け）
    //   -quit       : 処理が完了したら Unity を終了する
    //   -projectPath: プロジェクトのルートディレクトリを指定する
    //   -logFile    : ログの出力先ファイルパスを指定する
    String baseArgs = [
        '-batchmode',
        '-nographics',
        '-quit',
        "-projectPath \"${projectPath}\"",
        "-logFile \"${projectPath}/${args.logFile}\""
    ].join(' ')

    String fullCommand = "\"${args.executable}\" ${baseArgs} ${args.extraArgs}"

    if (isUnix()) {
        sh(script: fullCommand, label: "Unity: ${args.extraArgs}")
    } else {
        bat(script: fullCommand, label: "Unity: ${args.extraArgs}")
    }
}
