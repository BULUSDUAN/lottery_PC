import java.io.PrintStream;

public class AsmAopExample$Foo {

    public static void execute() {

        AsmAopExample.Monitor.start(); System.out.println("test changed method name");

        try {

            Thread.sleep(10L);

        }

        catch (InterruptedException e) {

            e.printStackTrace();

        }

        AsmAopExample.Monitor.end();

    }

}