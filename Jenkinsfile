pipeline {
    agent any
    
    environment {
        // To nazwa, pod jak¹ Docker bêdzie widzia³ te kontenery. 
        COMPOSE_PROJECT_NAME = 'helpdesk-system'
    }

    stages {
        stage('Sprz¹tanie') {
            steps {
                // Wy³¹czamy stare kontenery tego projektu (jeœli dzia³aj¹)
                sh 'docker-compose down || true'
            }
        }
        
        stage('Budowanie i Start') {
            steps {
                // Budujemy nowe obrazy i uruchamiamy w tle (-d)
                sh 'docker-compose up -d --build'
            }
        }
        
        stage('Weryfikacja') {
            steps {
                // Czekamy chwilê, ¿eby baza na pewno wsta³a
                sleep 10
                echo 'Aplikacja powinna dzia³aæ na porcie 8085!'
            }
        }
    }
}