下面我们就通过一个示例来对方法Foo.execute调用时动态注入方法执行前后的时间统计来统计方法的执行时间

[java]

import java.io.FileOutputStream;

import java.io.IOException;

import java.lang.reflect.InvocationTargetException;

import java.lang.reflect.Method;

import org.objectweb.asm.ClassReader;

import org.objectweb.asm.ClassVisitor;

import org.objectweb.asm.ClassWriter;

import org.objectweb.asm.MethodVisitor;

import org.objectweb.asm.Opcodes;

public class AsmAopExample extends ClassLoader implements Opcodes {

    public static class Foo {

public static void execute() {

        System.out.println("test changed method name");

        try {

            Thread.sleep(10);

        } catch (InterruptedException e) {

            // TODO Auto-generated catch block

            e.printStackTrace();

        }

    }

}

public static class Monitor {

    static long start = 0;

    public static void start() {

        start = System.currentTimeMillis();

    }

    public static void end() {

        long end = System.currentTimeMillis();

        System.out.println("execute method use time :" + (end - start));

    }

}

public static void main(String[] args) throws IOException, IllegalArgumentException, SecurityException, IllegalAccessException, InvocationTargetException {

    ClassReader cr = new ClassReader(Foo.class.getName());

    ClassWriter cw = new ClassWriter(cr, ClassWriter.COMPUTE_MAXS);

    ClassVisitor cv = new MethodChangeClassAdapter(cw);

    cr.accept(cv, Opcodes.ASM4);

    // gets the bytecode of the Example class, and loads it dynamically

    byte[] code = cw.toByteArray();

    AsmAopExample loader = new AsmAopExample();

    Class exampleClass = loader.defineClass(Foo.class.getName(), code, 0, code.length);

    for (Method method: exampleClass.getMethods()) {

        System.out.println(method);

    }

    exampleClass.getMethods()[0].invoke(null, null);  //調用execute，修改方法內容

    // gets the bytecode of the Example class, and loads it dynamically

    FileOutputStream fos = new FileOutputStream("e:\\logs\\Example.class");

    fos.write(code);

    fos.close();

}

static class MethodChangeClassAdapter extends ClassVisitor implements Opcodes {

    public MethodChangeClassAdapter(final ClassVisitor cv) {

        super(Opcodes.ASM4, cv);

    }

    @Override

    public MethodVisitor visitMethod(

        int access,

        String name,

        String desc,

        String signature,

        String[] exceptions) {

        if ("execute".equals(name))  //此处的execute即为需要修改的方法  ，修改方法內容

        {

            MethodVisitor mv = cv.visitMethod(access, name, desc, signature, exceptions);//先得到原始的方法

            MethodVisitor newMethod = null;

            newMethod = new AsmMethodVisit(mv); //访问需要修改的方法

            return newMethod;

        }

        return null;

    }

}

static class AsmMethodVisit extends MethodVisitor {

    public AsmMethodVisit(MethodVisitor mv) {

        super(Opcodes.ASM4, mv);

    }

    @Override

    public void visitCode() {

        //此方法在访问方法的头部时被访问到，仅被访问一次

        visitMethodInsn(Opcodes.INVOKESTATIC, Monitor.class.getName(), "start", "()V");

        super.visitCode();

    }

    @Override

    public void visitInsn(int opcode) {

        //此方法可以获取方法中每一条指令的操作类型，被访问多次

        //如应在方法结尾处添加新指令，则应判断：

        if (opcode == Opcodes.RETURN) {

            visitMethodInsn(Opcodes.INVOKESTATIC, Monitor.class.getName(), "end", "()V");

        }

        super.visitInsn(opcode);

    }

}

}

输出：

[java]

public static void AsmAopExample$Foo.execute()

public native int java.lang.Object.hashCode()

public final native java.lang.Class java.lang.Object.getClass()

public final void java.lang.Object.wait(long,int) throws java.lang.InterruptedException

public final void java.lang.Object.wait() throws java.lang.InterruptedException

public final native void java.lang.Object.wait(long) throws java.lang.InterruptedException

public boolean java.lang.Object.equals(java.lang.Object)

public java.lang.String java.lang.Object.toString()

public final native void java.lang.Object.notify()

public final native void java.lang.Object.notifyAll()

test changed method name

execute method use time :10

可以看到在execute方法中sleep 10ms，这里打印出来也是10ms，这里是在execute方法执行前先调用monitor.start()方法，方法返回是调用monitor的end方法，从而达到统计的功能，不过这里只是一个示例，如果要统计每个方法的执行时间，统计并发进行方法统计时这里当然要进行扩展，不过思路差不多就是这样。

我们查下最终生成的Foo类的class文件通过反射后的源代码：

[java]

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